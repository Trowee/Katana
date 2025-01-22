using Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PlayerCamera
{
    [RequireComponent(typeof(Volume))]
    public class DynamicDOF : MonoBehaviour
    {
        private static Camera Camera => PlaySceneManager.CameraManager.Camera;
        
        [SerializeField] private DepthOfField _dof;
        [SerializeField] private float _gaussianSize;
        [SerializeField] private LayerMask _layerMask;

        private void Reset()
        {
            // Get volume and add dof if it doesn't exist
            var volume = GetComponent<Volume>();
            if (!volume.sharedProfile.TryGet(out _dof)) _dof = volume.profile.Add<DepthOfField>();
        }

        private void Update()
        {
            // Return if dof doesn't exist
            if (_dof == null) return;
            
            // Set the default distance to 10000(basically no dof)
            float distance = 1000;
            
            // Create a ray from the camera center, check for hits and update the distance accordingly
            var ray = Camera.ScreenPointToRay(new(Screen.width / 2f, Screen.height / 2f));
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerMask)) distance = hit.distance;
            
            // Update the dof component
            _dof.focusDistance.value = distance;
            _dof.gaussianStart.value = distance - _gaussianSize;
            _dof.gaussianEnd.value   = distance + _gaussianSize;
        }
    }
}