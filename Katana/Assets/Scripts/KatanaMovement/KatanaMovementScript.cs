using System;
using System.Collections;
using Core;
using NnUtils.Scripts;
using TimeScale;
using UnityEngine;

namespace KatanaMovement
{
    [RequireComponent(typeof(Rigidbody))]
    public class KatanaMovementScript : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Transform _model;

        [Header("Tilt")]
        [SerializeField] private float _tiltSpeed = 90;
       
        [Header("Jump")]
        [SerializeField] private Vector2 _jumpForce = new(10, 15);
        [SerializeField] private float _jumpRotation = 10;
        [SerializeField] private TimeScaleKeys _jumpTimeScale;
        
        [Header("Dash")]
        [SerializeField] private float _dashForce = 30;
        [SerializeField] private TimeScaleKeys _dashTimeScale;

        [Header("Strike")]
        [SerializeField] private float _strikeTransitionTime = 5;
        [SerializeField] private float _strikeHeight = 100;
        [SerializeField] private float _strikeForce = 100;
        [SerializeField] private TimeScaleKeys _strikeHoverTimeScale;
        [SerializeField] private TimeScaleKeys _strikeImpactTimeScale;

        private void Reset()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Tilt();
            if (Input.GetKeyDown(KeyCode.Space)) Jump();
            if (Input.GetKeyDown(KeyCode.Mouse0)) Dash();
            if (Input.GetKeyDown(KeyCode.Mouse1)) Strike();
        }

        private Vector3 GetForward()
        {
            var rot = transform.rotation.eulerAngles;
            var noX = Quaternion.Euler(0, rot.y, rot.z);
            return noX * Vector3.forward;
        }

        private void Tilt()
        {
            var amount = Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime * _tiltSpeed;

            // Get the current rotation
            var currentRotation = transform.localRotation;

            // Define the tilt as a rotation around the Y axis
            var tilt = Quaternion.AngleAxis(amount, Vector3.up);

            // Combine the rotations
            transform.localRotation = currentRotation * tilt;
        }

        private void Jump()
        {
            if (_strikeRoutine != null) return;
            
            // Store forward
            var forward = GetForward();
            
            // Calculate forces
            var zForce = _jumpForce.x * forward;
            var force = new Vector3(0, _jumpForce.y) + zForce;
            
            // Add force and rotation
            _rb.AddForce(force, ForceMode.Impulse);
            _rb.AddTorque(Vector3.right * _jumpRotation, ForceMode.Impulse);
            
            // Apply timescale changes
            GameManager.TimeScaleManager.UpdateTimeScale(_jumpTimeScale);
        }

        private void Dash()
        {
            if (_strikeRoutine != null) return;
            
            // Reset velocity
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            
            // Add force
            _rb.AddForce(-transform.up * _dashForce, ForceMode.Impulse);
            
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
            // Disable gravity
            _rb.useGravity = false;
            
            // Reset velocity
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            
            // Adjust transform
            transform.localPosition += Vector3.up * 100;
            transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
            
            yield return new WaitForSeconds(5);
            _strikeRoutine = null;
        }
    }
}