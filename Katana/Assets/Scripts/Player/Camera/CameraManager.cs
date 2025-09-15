using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArtificeToolkit.Attributes;
using NnUtils.Modules.Easings;
using NnUtils.Scripts;
using UnityCommunity.UnitySingleton;
using UnityEngine;

namespace Assets.Scripts.Player.Camera
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        [SerializeField, Required] private Transform _cameraHolder;
        public static Transform CameraHolder => Instance._cameraHolder;
        [SerializeField, Required] private UnityEngine.Camera _worldCamera;
        public static UnityEngine.Camera WorldCamera => Instance._worldCamera;
        [SerializeField, Required] private UnityEngine.Camera _playerCamera;
        public static UnityEngine.Camera PlayerCamera => Instance._playerCamera;

        [SerializeField]
        private CameraAnimationSettings _defaultAnimationSettings = new(0.5f, EasingType.ExpoOut);
        public static CameraAnimationSettings DefaultAnimationSettings =>
           Instance._defaultAnimationSettings;

        #region Handlers

        private CameraHandlerScript _currentHandler;
        public static CameraHandlerScript CurrentHandler => Instance._currentHandler;
        public static Perspective CurrentPerspective => CurrentHandler.Perspective;

        [SerializeField]
        [ValidateInput(nameof(ValidateHandlers))]
        private List<CameraHandlerScript> _handlers = new();
        public static List<CameraHandlerScript> Handlers => Instance._handlers;

        private bool ValidateHandlers(ref string msg)
        {
            if (Handlers.Count < 1)
            {
                msg = "There must be at least 1 Camera Handler";
                return false;
            }

            HashSet<CameraHandlerScript> handlers = new();
            foreach (var handler in Handlers)
            {
                var duplicate = handlers.FirstOrDefault(x => x.Perspective == handler.Perspective);
                if (duplicate == null)
                {
                    handlers.Add(handler);
                    continue;
                }

                msg = $"Camera Handlers can't share Perspectives:\n{handler}\n{duplicate}";
                return false;
            }

            return true;
        }

        #endregion

        protected override void OnMonoSingletonCreated()
        {
            base.OnMonoSingletonCreated();
            SwitchToDefaultHandler();
        }

        public static void SwitchToDefaultHandler()
        {
            if (Handlers.Count < 1)
            {
                Debug.LogError("No Camera Handlers Found");
                return;
            }

            SwitchCameraHandler(Handlers[0].Perspective, new());
        }

        public static void SwitchToNextHandler(
            CameraAnimationSettings animationSettings = null, bool unscaled = true)
        {
            int index = Handlers.IndexOf(CurrentHandler) + 1;
            index = index >= Handlers.Count ? 0 : index;
            SwitchCameraHandler(Handlers[index].Perspective, animationSettings, unscaled);
        }

        public static void SwitchCameraHandler(Perspective perspective,
            CameraAnimationSettings animationSettings = null, bool unscaled = true)
        {
            Instance._currentHandler = Handlers.FirstOrDefault(x => x.Perspective == perspective);
            if (!CurrentHandler) return;

            CameraHolder.parent = CurrentHandler.transform;
            animationSettings ??= DefaultAnimationSettings;
            Instance.RestartRoutine(ref Instance._switchCameraRoutine,
                Instance.SwitchCameraRoutine(animationSettings, unscaled));
        }

        private Coroutine _switchCameraRoutine;
        private IEnumerator SwitchCameraRoutine(
            CameraAnimationSettings animationSettings, bool unscaled)
        {
            var startPos = CameraHolder.localPosition;
            var startRot = CameraHolder.transform.localRotation;

            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = Misc.Tween(ref lerpPos, animationSettings.Duration,
                    animationSettings.Easing, animationSettings.Curve, unscaled);
                CameraHolder.localPosition = Vector3.Lerp(startPos, Vector3.zero, t);
                CameraHolder.localRotation = Quaternion.Lerp(startRot, Quaternion.identity, t);
                yield return null;
            }

            _switchCameraRoutine = null;
        }
    }
}
