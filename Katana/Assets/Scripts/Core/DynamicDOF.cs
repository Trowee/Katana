using System.Collections;
using ArtificeToolkit.Attributes;
using NnUtils.Scripts;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Core
{
    [ExecuteAlways]
    [RequireComponent(typeof(Volume))]
    public class DynamicDOF : MonoBehaviour
    {
        private static Camera _cam;
        public static Camera Camera
        {
            get
            {
                if (_cam) return _cam;
                _cam = Camera.main;
                return _cam;
            }
        }

        private float _distance, _previousDistance, _targetDistance;

        [FoldoutGroup("Components"), SerializeField] private Transform _dofPoint;
        [FoldoutGroup("Components"), SerializeField] private DepthOfField _dof;
        
        [FoldoutGroup("Values"), SerializeField] private bool _followMouse;
        [FoldoutGroup("Values"), SerializeField] private LayerMask _layerMask;
        [FoldoutGroup("Values"), SerializeField] private float _defaultDistance = 100;
        [FoldoutGroup("Values"), SerializeField] private float _transitionTime = 1;
        [FoldoutGroup("Values"), SerializeField] private Easings.Type _transitionEasing = Easings.Type.ExpoOut;

        private void Reset()
        {
            // Get volume and add dof if it doesn't exist
            var volume = GetComponent<Volume>();
            if (!volume.sharedProfile.TryGet(out _dof)) _dof = volume.profile.Add<DepthOfField>();
            _dof.mode.value = DepthOfFieldMode.Bokeh;
        }

        private void Update()
        {
            if (!_dof) return;
            if (!_followMouse && !_dofPoint) return;

            var distance = _defaultDistance;

            // Create a ray and set the distance if there was a hit
            var ray = _followMouse
                ? Camera.ScreenPointToRay(Input.mousePosition)
                : new(_dofPoint.position, _dofPoint.forward);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerMask)) distance = hit.distance;
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
                var t = Misc.Tween(ref lerpPos, _transitionTime, _transitionEasing, true);
                _distance = Mathf.Lerp(_previousDistance, _targetDistance, t);
                _dof.focusDistance.value = _distance;
                yield return null;
            }
            _updateDOFRoutine = null;
        }
    }
}