using System.Collections;
using ArtificeToolkit.Attributes;
using NnUtils.Scripts;
using UnityEngine;

namespace Assets.Scripts.TimeScale
{
    public class TimeScaleManager : MonoBehaviour
    {
        [SerializeField] private TimeScaleKey _pauseKey;
        [SerializeField] private TimeScaleKey _unpauseKey;

        private float _fixedDeltaTime;

        private float _timeScale = 1;
        public float TimeScale
        {
            get => _timeScale;
            private set
            {
                if (Mathf.Approximately(_timeScale, value)) return;
                _timeScale = Mathf.Max(0, value);
                UpdateTimeScale();
            }
        }

        private float _pauseTimeScale = 1;
        public float PauseTimeScale
        {
            get => _pauseTimeScale;
            private set
            {
                if (Mathf.Approximately(_pauseTimeScale, value)) return;
                _pauseTimeScale = Mathf.Max(0, value);
                UpdateTimeScale();
            }
        }

        private int? _currentPriority;

        private void Start()
        {
            TimeScale = Time.timeScale;
            _fixedDeltaTime = Time.fixedDeltaTime;
        }

        private void UpdateTimeScale()
        {
            Time.timeScale = TimeScale * PauseTimeScale;
            Time.fixedDeltaTime = _fixedDeltaTime * Time.timeScale;
        }

        public void SetTimeScale(float timeScale, int priority = 0)
        {
            if (_currentPriority < priority) return;
            this.StopRoutine(ref _setTimeScaleRoutine);
            TimeScale = timeScale;
            _currentPriority = null;
        }

        public void SetTimeScale(TimeScaleKey timeScaleKey, int priority = 0)
            => SetTimeScale(new TimeScaleKeys(timeScaleKey), priority);

        public void SetTimeScale(TimeScaleKeys timeScales, int priority = 0)
        {
            if (_currentPriority.HasValue && _currentPriority < priority) return;
            _currentPriority = priority;
            this.RestartRoutine(ref _setTimeScaleRoutine, SetTimeScaleRoutine(timeScales, false));
        }

        [HorizontalGroup]
        [Button]
        public void Pause()
        {
            if (!Application.isPlaying) return;
            this.RestartRoutine(ref _setPauseTimeScaleRoutine,
                SetTimeScaleRoutine(new TimeScaleKeys(_pauseKey), true));
        }

        [HorizontalGroup]
        [Button]
        public void Unpause()
        {
            if (!Application.isPlaying) return;
            this.RestartRoutine(ref _setPauseTimeScaleRoutine,
                SetTimeScaleRoutine(new TimeScaleKeys(_unpauseKey), true));
        }

        private Coroutine _setTimeScaleRoutine;
        private Coroutine _setPauseTimeScaleRoutine;

        private IEnumerator SetTimeScaleRoutine(TimeScaleKeys timeScales, bool SetPauseTimeScale)
        {
            foreach (var timeScale in timeScales.Keys)
            {
                var startTimeScale = SetPauseTimeScale ? PauseTimeScale : TimeScale;

                float lerpPos = 0;
                while (lerpPos < 1)
                {
                    var t = Misc.Tween(ref lerpPos, timeScale.Time, timeScale.Easing, true,
                     multiplier: SetPauseTimeScale ? 1 : PauseTimeScale);
                    var ts = Mathf.Lerp(startTimeScale, timeScale.TimeScale, t);
                    if (SetPauseTimeScale) PauseTimeScale = ts;
                    else TimeScale = ts;
                    yield return null;
                }
            }

            if (!SetPauseTimeScale) _currentPriority = null;
        }
    }
}
