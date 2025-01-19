using System.Collections;
using Core;
using NnUtils.Scripts;
using TimeScale;
using UnityEngine;

namespace KatanaMovement
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovementScript : MonoBehaviour
    {
        private static Camera Cam => PlaySceneManager.CameraManager.Camera;
        
        /// Whether the player is currently using the strike ability
        [ReadOnly] public bool IsPerformingStrike;
        
        // Whether the player is performing the strike impact
        [ReadOnly] public bool IsPerformingStrikeImpact;
        
        [Header("Components")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _model;

        [Header("Tilt")]
        [SerializeField] private float _tiltSpeed = 90;
        [SerializeField] private float _tiltInvertPoint = 180;
       
        [Header("Flip")]
        [SerializeField] private Vector2 _flipForce = new(20, 25);
        [SerializeField] private float _flipRotation = 5;
        [SerializeField] private TimeScaleKeys _flipTimeScale;
        
        [Header("Dash")]
        [SerializeField] private float _dashForce = 30;
        [SerializeField] private TimeScaleKeys _dashTimeScale;

        [Header("Strike Hover")]
        [SerializeField] private float _strikeTransitionTime = 3;
        [SerializeField] private AnimationCurve _strikeTransitionCurve;
        [SerializeField] private float _strikeHoverHeight = 100;
        [SerializeField] private float _strikeHoverDuration = 5;
        [SerializeField] private float _strikeHoverTimeScale = 0.1f;

        [Header("Strike Impact")]
        [SerializeField] private float _camReturnDuration = 0.5f;
        [SerializeField] private Easings.Type _camReturnEasing = Easings.Type.ExpoOut;
        [SerializeField] private LayerMask _strikeLayerMask;
        [SerializeField] private float _strikeImpactForce = 200;
        [SerializeField] private TimeScaleKeys _strikeImpactTimeScale;
        [SerializeField] private TimeScaleKeys _postStrikeImpactTimeScale;

        private void Reset()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Tilt();
            if (Input.GetKeyDown(KeyCode.Space)) Flip();
            if (Input.GetKeyDown(KeyCode.Mouse0)) Dash();
            if (Input.GetKeyDown(KeyCode.S)) Strike();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (IsPerformingStrikeImpact)
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
                { StrikeImpact(); return; }
            }
            
        }

        private void Tilt()
        {
            if (IsPerformingStrike || IsPerformingStrikeImpact) return;

            var ea = transform.eulerAngles;
            var amount = Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime * _tiltSpeed;
            amount *= ea.x > 180 ? -1 : 1;
            amount *= ea.x < -180 ? -1 : 1;

            // Combine the rotations
            transform.Rotate(Vector3.up, amount, Space.World);
            transform.eulerAngles = new(ea.x, transform.eulerAngles.y, ea.z);
        }

        private void Flip()
        {
            if (IsPerformingStrike || IsPerformingStrikeImpact) return;
            
            // Store forward
            var forward = transform.up;
            
            // Calculate forces
            var zForce = _flipForce.x * forward;
            var force = new Vector3(0, _flipForce.y) + zForce;
            
            // Add force and rotation
            _rb.AddForce(force, ForceMode.Impulse);
            _rb.AddTorque(transform.right * _flipRotation, ForceMode.Impulse);
            
            // Apply timescale changes
            GameManager.TimeScaleManager.UpdateTimeScale(_flipTimeScale);
        }

        private void Dash()
        {
            if (IsPerformingStrike || IsPerformingStrikeImpact) return;
            
            // Reset velocity
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            
            // Add force
            _rb.AddForce(transform.up * _dashForce, ForceMode.Impulse);
            
            // Apply timescale changes
            GameManager.TimeScaleManager.UpdateTimeScale(_dashTimeScale);
        }

        private void Strike()
        {
            if (_strikeRoutine != null) return;
            this.RestartRoutine(ref _strikeRoutine, StrikeRoutine());
        }

        private Coroutine _strikeRoutine;

        private IEnumerator StrikeRoutine()
        {
            IsPerformingStrike = true;
            
            // Disable gravity
            _rb.useGravity = false;
            
            // Disable collider
            _collider.enabled = false;
            
            // Reset velocity
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            
            // Store transform
            var startPos = transform.localPosition;
            var targetPos = Vector3.up * _strikeHoverHeight;
            var startRot = transform.localRotation;
            var targetRot = Quaternion.Euler(180, startRot.eulerAngles.y, 0);

            // Store the timescale and lerp position
            var startTimeScale = Time.timeScale;
            float lerpPos = 0;
            
            // Start the camera switch and animation
            PlaySceneManager.CameraManager.SwitchCameraHandler("StrikeCameraHandler", _strikeTransitionTime, _strikeTransitionCurve, true);
            while (lerpPos < 1)
            {
                var t = _strikeTransitionCurve.Evaluate(Misc.Tween(ref lerpPos, _strikeTransitionTime, unscaled: true));
                transform.localPosition = Vector3.LerpUnclamped(startPos, targetPos, t);
                transform.localRotation = Quaternion.LerpUnclamped(startRot, targetRot, t);
                GameManager.TimeScaleManager.UpdateTimeScale(Mathf.LerpUnclamped(startTimeScale, _strikeHoverTimeScale, t));
                yield return null;
            }

            // Perform hover
            float timer = 0;
            while (timer < _strikeHoverDuration)
            {
                timer += Time.unscaledDeltaTime;
                PerformStrikeHover();
                if (Input.GetKeyDown(KeyCode.Mouse0)) break;
                yield return null;
            }

            // Return camera to the initial handler
            PlaySceneManager.CameraManager.SwitchCameraHandler("PlayerCameraHandler", _camReturnDuration, _camReturnEasing, true);
            yield return new WaitForSecondsRealtime(_camReturnDuration);
            
            PerformStrikeImpact();
            _strikeRoutine = null;
        }

        private void PerformStrikeHover()
        {
            // Perform the raycast
            var ray = Cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, _strikeLayerMask))
                return;

            // Rotate player towards the hit point
            transform.LookAt(hit.point, Vector3.down);
            transform.Rotate(90, 0, 0);
        }

        private void PerformStrikeImpact()
        {
            // Update values
            IsPerformingStrike       = false;
            IsPerformingStrikeImpact = true;
            _rb.useGravity           = true;
            _collider.enabled        = true;

            // Add strike to where the blade is pointing
            _rb.AddForce(transform.up * _strikeImpactForce, ForceMode.Impulse);
            GameManager.TimeScaleManager.UpdateTimeScale(_strikeImpactTimeScale);
        }

        private void StrikeImpact()
        {
            IsPerformingStrikeImpact = false;
            GameManager.TimeScaleManager.UpdateTimeScale(_postStrikeImpactTimeScale);
        }
    }
}