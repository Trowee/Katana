using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerCamera
{
    public class CameraManager : MonoBehaviour
    {
        private readonly HashSet<CameraHandlerScript> _handlers = new();
        public Camera Camera { get; private set; }
        public PlayerCameraScript PlayerCamera { get; private set; }

        private void Awake()
        {
            _handlers.AddRange(FindObjectsByType<CameraHandlerScript>(FindObjectsSortMode.None));
            Camera = Camera.main;
            PlayerCamera = Camera?.GetComponent<PlayerCameraScript>();
        }

        public void SwitchCamera(string cameraName)
        {
            var cam = _handlers.First(x => x.name == cameraName);
            
        }
    }
}