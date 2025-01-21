using Core;
using UnityEngine;

namespace PlayerCamera
{
    public class CameraHandlerScript : MonoBehaviour
    {
        private static Transform Player => PlaySceneManager.Player.transform;
        
        [Header("Position")]
        
        [Tooltip("Offset from the player")]
        [SerializeField] private Vector3 _offset;
        
        [Tooltip("Whether the camera position will be moved or translated after rotation")]
        [SerializeField] private bool _translateCameraPosition = true;
        
        [Tooltip("Whether the camera will be rotated like the player")]
        [SerializeField] private bool _rotateCamera;

        [Tooltip("Delta between the player rotation and camera rotation")]
        [SerializeField] private Vector3 _rotationOffset;

        private void Start()
        {
            PlaySceneManager.CameraManager.Handlers.Add(this);
        }

        private void Update()
        {
            // Move camera to the player's position to allow for proper offset after rotation
            transform.position = Player.position;
            if (_rotateCamera) Rotate();
            Offset();
        }

        private void OnDestroy()
        {
            if (PlaySceneManager.CameraManager == null) return;
            PlaySceneManager.CameraManager.Handlers.Remove(this);
        }

        private void Offset()
        {
            // Apply offset to the camera
            if (_translateCameraPosition) transform.Translate(_offset);
            else transform.localPosition += _offset;
        }

        private void Rotate()
        {
            transform.localRotation = Player.localRotation * Quaternion.Euler(_rotationOffset);
        }
    }
}