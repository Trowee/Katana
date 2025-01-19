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
        private bool _isStriking;
        
        // Whether the player is performing the strike impact
        private bool _isPerformingStrikeImpact;
        
        [Header("Components")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _model;

        [Header("Tilt")]
        [SerializeField] private float _tiltSpeed = 90;
        [SerializeField] private float _tiltInvertPoint = 180;
       
        [Header("Jump")]
        [SerializeField] private Vector2 _jumpForce = new(10, 15);
        [SerializeField] private float _jumpRotation = 10;
        [SerializeField] private TimeScaleKeys _jumpTimeScale;
        
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
            if (Input.GetKeyDown(KeyCode.Space)) Jump();
            if (Input.GetKeyDown(KeyCode.Mouse0)) Dash();
            if (Input.GetKeyDown(KeyCode.S)) Strike();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_isPerformingStrikeImpact)
            {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Default"))
                { StrikeImpact(); return; }
            }
            
        }

        private Vector3 GetForward()
        {
            var rot = transform.rotation.eulerAngles;
            var noX = Quaternion.Euler(0, rot.y, rot.z);
            return noX * Vector3.forward;
        }

        private void Tilt()
        {
            if (_isStriking || _isPerformingStrikeImpact) return;

            var ea = transform.eulerAngles;
            var amount = Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime * _tiltSpeed;
            amount *= ea.x > 180 ? -1 : 1;
            amount *= ea.x < -180 ? -1 : 1;

            // Combine the rotations
            transform.Rotate(Vector3.up, amount, Space.World);
            transform.eulerAngles = new(ea.x, transform.eulerAngles.y, ea.z);
        }

        private void Jump()
        {
            if (_isStriking || _isPerformingStrikeImpact) return;
            
            // Store forward
            var forward = transform.up;
            
            // Calculate forces
            var zForce = _jumpForce.x * forward;
            var force = new Vector3(0, _jumpForce.y) + zForce;
            
            // Add force and rotation
            _rb.AddForce(force, ForceMode.Impulse);
            _rb.AddTorque(transform.right * _jumpRotation, ForceMode.Impulse);
            
            // Apply timescale changes
            GameManager.TimeScaleManager.UpdateTimeScale(_jumpTimeScale);
        }

        private void Dash()
        {
            if (_isStriking || _isPerformingStrikeImpact) return;
            
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
            _isStriking = true;
            
            // Disable gravity
            _rb.useGravity = false;
            
            // Disable collider
            _collider.enabled = false;
            
            // Reset velocity
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            
            // Store transform
            var startPos = transform.localPosition;
            var targetPos = startPos + Vector3.up * _strikeHoverHeight;
            var startRot = transform.localRotation;
            var targetRot = Quaternion.Euler(0, startRot.eulerAngles.y, 0);

            var startTimeScale = Time.timeScale;
            float lerpPos = 0;

            while (lerpPos < 1)
            {
                var t = _strikeTransitionCurve.Evaluate(Misc.Tween(ref lerpPos, _strikeTransitionTime, unscaled: true));
                transform.localPosition = Vector3.LerpUnclamped(startPos, targetPos, t);
                transform.localRotation = Quaternion.LerpUnclamped(startRot, targetRot, t);
                GameManager.TimeScaleManager.UpdateTimeScale(Mathf.LerpUnclamped(startTimeScale, _strikeHoverTimeScale, t));
                yield return null;
            }

            float timer = 0;
            
            while (timer < _strikeHoverDuration)
            {
                timer += Time.unscaledDeltaTime;
                PerformStrikeHover();
                if (Input.GetKeyDown(KeyCode.Mouse0)) { PerformStrikeImpact(); break; } 
                yield return null;
            }
            
            PerformStrikeImpact();
            _strikeRoutine = null;
        }

        private void PerformStrikeHover()
        {
            var ray = Cam.ScreenPointToRay(Input.mousePosition);

            // Perform the raycast
            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, _strikeLayerMask))
                return;

            // Orient the bottom side of the object towards the hit point
            transform.LookAt(hit.point, Vector3.up);
            transform.Rotate(-90, 0, 0); // Adjust rotation so the bottom side faces the target
        }

        private void PerformStrikeImpact()
        {
            // Update values
            _isStriking               = false;
            _isPerformingStrikeImpact = true;
            _rb.useGravity            = true;
            _collider.enabled         = true;
            
            // Add strike to where the blade is pointing
            _rb.AddForce(-transform.up * _strikeImpactForce, ForceMode.Impulse);
            GameManager.TimeScaleManager.UpdateTimeScale(_strikeImpactTimeScale);
        }

        private void StrikeImpact()
        {
            _isPerformingStrikeImpact = false;
            GameManager.TimeScaleManager.UpdateTimeScale(_postStrikeImpactTimeScale);
        }
    }
}