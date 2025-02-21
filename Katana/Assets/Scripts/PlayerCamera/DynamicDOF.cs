using System.Collections;
using Assets.Scripts.Core;
using NnUtils.Scripts;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.PlayerCamera
{
    [ExecuteAlways]
    [RequireComponent(typeof(Volume))]
    public class DynamicDOF : MonoBehaviour
    {
        private static Camera _cam;
        private static Camera Camera => _cam ??= Camera.main;

        private float _distance, _previousDistance, _targetDistance;

        [Header("Components")]
        [SerializeField] private bool _followMouse;
        [SerializeField] private Transform _dofPoint;
        [SerializeField] private DepthOfField _dof;
        
        [Header("Values")]
        [Tooltip("Used if raycast didn't hit anything")]
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private float _defaultDistance = 100;
        [SerializeField] private float _transitionTime = 1;
        [SerializeField] private Easings.Type _transitionEasing = Easings.Type.ExpoOut;

        private void Reset()
        {
            // Get volume and add dof if it doesn't exist
            var volume = GetComponent<Volume>();
            if (!volume.sharedProfile.TryGet(out _dof)) _dof = volume.profile.Add<DepthOfField>();
            _dof.mode.value = DepthOfFieldMode.Bokeh;
        }

        private void Update()
        {
            // Return if dof doesn't exist
            if (!_dof) return;
            
            // Return if not following the mouse and dof point is not set
            if (!_followMouse && !_dofPoint) return;

            // Set to default distance
            var distance = _defaultDistance;

            // Create a ray and set the distance if there was a hit
            var ray = _followMouse
                ? Camera.ScreenPointToRay(Input.mousePosition)
                : new(_dofPoint.position, _dofPoint.forward);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerMask)) distance = hit.distance;

            // Check if object distance is approx the same and return
            if (Mathf.Approximately(distance, _targetDistance)) return;

            // Update distance values and restart the transition routine
            _previousDistance = _distance;
            _targetDistance = distance;
            this.RestartRoutine(ref _updateDOFRoutine, UpdateDOFRoutine());
        }

        private Coroutine _updateDOFRoutine;
        private IEnumerator UpdateDOFRoutine()
        {
            float lerpPos = 0;
            
            while (lerpPos < 1)
            {
                // Update T
                var t = Misc.Tween(ref lerpPos, _transitionTime, _transitionEasing, true);
                _distance = Mathf.Lerp(_previousDistance, _targetDistance, t);

                // Update the dof component
                _dof.focusDistance.value = _distance;

                // Wait for the next frame
                yield return null;
            }

            _updateDOFRoutine = null;
        }
    }
}