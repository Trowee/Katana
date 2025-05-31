using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArtificeToolkit.Attributes;
using NnUtils.Scripts;
using UnityEngine;

namespace Assets.Scripts.Player.Camera
{
    public class CameraManager : MonoBehaviour
    {
        [Required] public Transform CameraHolder;
        [Required] public UnityEngine.Camera Camera;
        [Required] public UnityEngine.Camera PlayerCamera;
        
        #region Handlers
        /*[ListViewSettings(ShowAddRemoveFooter = false, ShowBoundCollectionSize = false),
         OnListViewChanged(
            OnItemChanged = nameof(HandleHandlersItemChanged),
            OnItemIndexChanged = nameof(HandleHandlersIndexChanged)
            )]*/
        public List<CameraHandlerScript> Handlers = new();

        private void HandleHandlersItemChanged(int index, CameraHandlerScript handler)
        {
            if (index != 0) return;
            SwitchToDefaultHandler();
        }

        private void HandleHandlersIndexChanged(int before, int after)
        {
            if (before != 0 && after != 0) return;
            SwitchToDefaultHandler();
        }
        #endregion
        
        private void Start()
        {
            SwitchToDefaultHandler();
        }

        private void SwitchToDefaultHandler()
        {
            if (Handlers.Count < 1) return;
            SwitchCameraHandler(Handlers[0].Perspective, 0, Easings.Type.None);
        }
        
        public void SwitchCameraHandler(Perspective perspective, float duration = 0,
            Easings.Type easing = Easings.Type.None, bool unscaled = false)
            => SwitchCameraHandler(perspective, duration, easing, null, unscaled);

        public void SwitchCameraHandler(Perspective perspective, float duration = 0,
            AnimationCurve curve = null, bool unscaled = false)
            => SwitchCameraHandler(perspective, duration, Easings.Type.None, curve, unscaled);

        private void SwitchCameraHandler(Perspective perspective, float duration,
            Easings.Type easing, AnimationCurve curve, bool unscaled)
        {
            var handler = Handlers.FirstOrDefault(x => x.Perspective == perspective);
            if (!handler) return;

            // Change the cam parent and switch
            CameraHolder.parent = handler.transform;
            this.RestartRoutine(ref _switchCameraRoutine,
                SwitchCameraRoutine(duration, easing, curve, unscaled));
        }

        private Coroutine _switchCameraRoutine;
        private IEnumerator SwitchCameraRoutine(float duration, Easings.Type easing,
            AnimationCurve curve, bool unscaled)
        {
            var startPos = CameraHolder.localPosition;
            var startRot = CameraHolder.transform.localRotation;

            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = Misc.Tween(ref lerpPos, duration, easing, unscaled);
                if (curve != null) t = curve.Evaluate(t);

                CameraHolder.localPosition = Vector3.Lerp(startPos, Vector3.zero, t);
                CameraHolder.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, t);

                yield return null;
            }

            _switchCameraRoutine = null;
        }
    }
}