using Alchemy.Inspector;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Player.Camera
{
    [ExecuteAlways]
    public class CameraHandlerScript : MonoBehaviour
    {
        private static Transform Player => ColosseumSceneManager.Player.transform;
        
        public Perspective Perspective;
        [SerializeField] private LayerMask _cameraCollisionMask;

        [Tooltip("Offset from the player")]
        [FoldoutGroup("Position"), SerializeField] private Vector3 _offset;
        [Tooltip("Whether offset is applied locally or globally")]
        [FoldoutGroup("Position"), SerializeField] private bool _localOffset;
        [Tooltip("If enabled, offset will be applied according to the rotation")]
        [FoldoutGroup("Position"), SerializeField] private bool _translateOffset;

        [Tooltip("Whether the camera will follow player's rotation")]
        [FoldoutGroup("Rotation"), SerializeField] private bool _rotateCamera;
        [Tooltip("Delta between the player rotation and camera rotation")]
        [FoldoutGroup("Rotation"), SerializeField] private Vector3 _rotationOffset;

        private void OnEnable() => AddToSceneManager();

        private void Update()
        {
            // Move camera to the player's position to allow for proper offset after rotation
            transform.position = Player.position;
            if (_rotateCamera) Rotate();
            Offset();
        }
        
        [HorizontalGroup, Button]
        private void AddToSceneManager()
        {
            var handlers = ColosseumSceneManager.CameraManager.Handlers;
            if (!handlers.Contains(this)) ColosseumSceneManager.CameraManager.Handlers.Add(this);
        }
        
        [HorizontalGroup, Button]
        private void RemoveFromSceneManager()
        {
            var handlers = ColosseumSceneManager.CameraManager.Handlers;
            if (handlers.Contains(this)) ColosseumSceneManager.CameraManager.Handlers.Remove(this);
        }

        private void Offset()
        {
            var parent = transform.parent ? transform.parent : transform;
            var offset = _localOffset ? parent.TransformPoint(_offset) - Player.position : _offset;

            var desiredPos = transform.position +
                             (_translateOffset ? transform.TransformVector(offset) : offset);
            var dir = desiredPos - Player.position;
            var distance = dir.magnitude;

            // Set the camera handler position
            if (Physics.Raycast(Player.position, dir.normalized, out var hit, distance,
                    _cameraCollisionMask))
            {
                var targetPos = hit.point - dir.normalized * 0.1f;
                if (_localOffset) transform.localPosition = parent.InverseTransformPoint(targetPos);
                else transform.position                   = targetPos;
            }
            else
            {
                if (_localOffset)
                    transform.localPosition = parent.InverseTransformPoint(desiredPos);
                else transform.position     = desiredPos;
            }
        }

        private void Rotate()
        {
            transform.localRotation = Player.localRotation * Quaternion.Euler(_rotationOffset);
        }
    }
}