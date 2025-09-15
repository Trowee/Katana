using ArtificeToolkit.Attributes;
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

        private void OnEnable() => AddToSceneManager();

        private void Update()
        {
            // Move camera to the player's position to allow for proper offset
            transform.position = Player.position;
            Offset();
        }

        [HorizontalGroup("Management"), Button]
        private void AddToSceneManager()
        {
            var handlers = CameraManager.Handlers;
            if (!handlers.Contains(this)) CameraManager.Handlers.Add(this);
        }

        [HorizontalGroup("Management"), Button]
        private void RemoveFromSceneManager()
        {
            var handlers = CameraManager.Handlers;
            if (handlers.Contains(this)) CameraManager.Handlers.Remove(this);
        }

        [HorizontalGroup("Switching"), Button]
        private void SiwtchToPreviousHandler() => CameraManager.SwitchToPreviousHandler();

        [HorizontalGroup("Switching"), Button]
        private void SiwtchToThisHandler() => CameraManager.SwitchCameraHandler(Perspective);

        [HorizontalGroup("Switching"), Button]
        private void SiwtchToNextHandler() => CameraManager.SwitchToNextHandler();

        private void Offset()
        {
            var parent = transform.parent ? transform.parent : transform;
            var offset = _localOffset ? parent.TransformPoint(_offset) - Player.position : _offset;

            var desiredPos = transform.position + offset;
            var dir = desiredPos - Player.position;
            var distance = dir.magnitude;

            // Set the camera handler position
            if (Physics.Raycast(Player.position, dir.normalized, out var hit, distance,
                    _cameraCollisionMask))
            {
                var targetPos = hit.point - dir.normalized * 0.1f;
                if (_localOffset) transform.localPosition = parent.InverseTransformPoint(targetPos);
                else transform.position = targetPos;
            }
            else
            {
                if (_localOffset)
                    transform.localPosition = parent.InverseTransformPoint(desiredPos);
                else transform.position = desiredPos;
            }
        }
    }
}
