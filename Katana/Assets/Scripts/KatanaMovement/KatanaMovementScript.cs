using System;
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
       
        [Header("Jump")]
        [SerializeField] private Vector2 _jumpForce;
        [SerializeField] private float _jumpRotation;
        [SerializeField] private TimeScaleKeys _jumpTimeScale;
        
        [Header("Dash")]
        [SerializeField] private float _dashForce;
        [SerializeField] private TimeScaleKeys _dashTimeScale;

        [Header("Tilt")]
        [SerializeField] private float _tiltAmount;
        [SerializeField] private float _maxTilt;
        private float _defaultTilt, _currentTilt;

        private void Reset()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            _defaultTilt = _model.localRotation.eulerAngles.z;
        }

        private void Update()
        {
            Tilt();
            if (Input.GetKeyDown(KeyCode.Space)) Jump();
            if (Input.GetKeyDown(KeyCode.Mouse0)) Dash();
        }

        private Vector3 GetForward()
        {
            var rot = transform.rotation.eulerAngles;
            var noX = Quaternion.Euler(0, rot.y, rot.z);
            return noX * Vector3.forward;
        }

        private void Jump()
        {
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
            // Reset velocity
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            
            // Add force
            _rb.AddForce(-transform.up * _dashForce, ForceMode.Impulse);
            
            // Apply timescale changes
            GameManager.TimeScaleManager.UpdateTimeScale(_dashTimeScale);
        }

        private void Tilt()
        {
            float amount = Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime * _tiltAmount;

            // Get the current rotation
            Quaternion currentRotation = transform.localRotation;

            // Define the tilt as a rotation around the Z axis
            Quaternion tilt = Quaternion.AngleAxis(amount, Vector3.up);

            // Combine the rotations
            transform.localRotation = currentRotation * tilt;
        }
        
    }
}