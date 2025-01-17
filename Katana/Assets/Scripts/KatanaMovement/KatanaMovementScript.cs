using NnUtils.Scripts;
using TimeScale;
using UnityEngine;

namespace KatanaMovement
{
    [RequireComponent(typeof(Rigidbody))]
    public class KatanaMovementScript : MonoBehaviour
    {
        [ReadOnly] [SerializeField] private Rigidbody _rb;
        [SerializeField] private Transform _forcePoint;
        
        [Header("Jump")]
        [SerializeField] private Vector2 _jumpForce;
        [SerializeField] private float _jumpRotation;
        [SerializeField] private TimeScaleKeys _jumpTimeScale;
        
        [Header("Dash")]
        [SerializeField] private float _dashForce;
        [SerializeField] private TimeScaleKeys _dashTimeScale;

        private void Reset()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) Jump();
            if (Input.GetKeyDown(KeyCode.D)) Dash();
        }

        private void Jump()
        {
            var forward = GetForward();
            var zForce = _jumpForce.x * forward;
            var force = new Vector3(0, _jumpForce.y) + zForce;
            _rb.AddForce(force, ForceMode.Impulse);
            _rb.AddTorque(Vector3.right * _jumpRotation, ForceMode.Impulse);
        }

        private void Dash()
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.AddForce(-transform.up * _dashForce, ForceMode.Impulse);
        }

        private Vector3 GetForward()
        {
            var rot = transform.rotation.eulerAngles;
            var noX = Quaternion.Euler(0, rot.y, rot.z);
            return noX * Vector3.forward;
        }
    }
}