using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioManager.Effects;
using UnityEngine;

namespace AudioManager
{
    public class AudioManagerItem : MonoBehaviour
    {
        public enum TweenType
        {
            Volume,
            Pitch
        }

        public AudioManager Manager;
        
        public AudioManagerKey Key;
        
        /// AudioItem this Item was created from
        public AudioItem OriginalAudioItem;

        /// Current AudioItem
        public AudioItem AudioItem;

        public AudioSource Source;

        /// Whether item is paused
        public bool Paused { get; private set; }

        /// Unscaled Pitch
        public float Pitch { get; private set; } = 1;

        public readonly Dictionary<Type, HashSet<AudioEffect>> EffectsByType = new()
        {
            { typeof(AudioChorusFilter), new() },
            { typeof(AudioDistortionFilter), new() },
            { typeof(AudioEchoFilter), new() },
            { typeof(AudioHighPassFilter), new() },
            { typeof(AudioLowPassFilter), new() },
            { typeof(AudioReverbFilter), new() }
        };

        private readonly Dictionary<TweenType, Coroutine> _tweenRoutines = new();

        private void Update()
        {
            if (Source.isPlaying) Source.pitch = this.Scaled() ? Pitch * Time.timeScale : Pitch;
        }

        private void OnDestroy() => Manager.RemoveItem(this);

        public AudioManagerItem ApplySettings(bool initial = false)
        {
            OriginalAudioItem.ApplySettingsToSource(Source);
            if (AudioItem.OverrideSettings)
                if (initial || AudioItem.ReloadSettingsEveryPlay)
                    AudioItem.ApplySettingsToSource(Source);

            // Update pitch once before playing
            Pitch = Source.pitch;
            Source.pitch = this.Scaled() ? Pitch * Time.timeScale : Pitch;

            return this;
        }

        public AudioManagerItem ApplyEffects()
        {
            if (OriginalAudioItem.OverrideEffects)
                OriginalAudioItem.Effects.ApplyEffects(this);
            else OriginalAudioItem.Effects.ClearEffects(this);

            if (AudioItem.OverrideEffects)
            {
                if (OriginalAudioItem != AudioItem)
                    AudioItem.Effects.ApplyEffects(this);
            }
            else AudioItem.Effects.ClearEffects(this);

            foreach (var ec in EffectsByType)
                if (EffectsByType[ec.Key].Count < 1 &&
                    gameObject.TryGetComponent(ec.Key, out var effect))
                    DestroyImmediate(effect);

            return this;
        }

        public AudioManagerItem Play()
        {
            if (Paused) return Resume();
            if (PlayRoutine != null) Stop();
            PlayRoutine = StartCoroutine(PlayCoroutine());
            return this;
        }

        public AudioManagerItem Stop(bool finished = false)
        {
            if (PlayRoutine != null)
            {
                Source.Stop();
                StopCoroutine(PlayRoutine);
                PlayRoutine = null;
                Paused = false;
            }

            foreach (var routine in _tweenRoutines.Where(routine => routine.Value != null))
                StopCoroutine(routine.Value);

            if (!finished) return this;

            if (AudioItem.DestroyTargetOnFinished) DestroyImmediate(gameObject);
            else if (AudioItem.DestroySourceOnFinished)
            {
                DestroyEffects();
                DestroyImmediate(Source);
                DestroyImmediate(this);
            }

            return this;
        }

        private void DestroyEffects()
        {
            foreach (var (type, effects) in EffectsByType)
            {
                effects.Clear();
                if (gameObject.TryGetComponent(type, out var effect))
                    DestroyImmediate(effect);
            }
        }

        public AudioManagerItem Pause()
        {
            if (Paused) return this;
            Paused = true;
            Source.Pause();
            return this;
        }

        public AudioManagerItem Resume()
        {
            if (!Paused) return this;
            Paused = false;
            Source.UnPause();
            return this;
        }

        public Coroutine PlayRoutine;

        private IEnumerator PlayCoroutine()
        {
            var fadeInTime = this.FadeInTime();
            var fadeInEasing = this.FadeInEasing();
            var fadeInScale = this.FadeInScale();
            var fadeInScaleWithPitch = this.FadeInScaleWithPitch();

            if (this.FadeIn())
                TweenVolume(0, Source.volume, fadeInTime, out _, fadeInEasing,
                            fadeInScale, fadeInScaleWithPitch);
            Source.Play();

            if (this.FadeOut())
            {
                var fadeOutTime = this.FadeOutTime();
                var fadeOutEasing = this.FadeOutEasing();
                var fadeOutScale = this.FadeOutScale();
                var fadeOutScaleWithPitch = this.FadeOutScaleWithPitch();

                int FadeOutSample() =>
                    Source.clip.samples -
                    Mathf.RoundToInt(Source.clip.frequency *
                                     (fadeOutTime / (fadeOutScale ? Time.timeScale : 1) /
                                      (fadeOutScaleWithPitch ? Pitch : 1) +
                                      0.05f));

                yield return new WaitUntil(() => Source.timeSamples >= FadeOutSample());
                yield return TweenVolume(Source.volume, 0, fadeOutTime, out _,
                                         fadeOutEasing, fadeOutScale, fadeOutScaleWithPitch);
            }

            yield return new WaitWhile(() => Source.isPlaying);
            Stop(true);
        }

        public AudioManagerItem TweenVolume(float from, float to,
                                            float duration, out Coroutine routine,
                                            Easings.Type easing = Easings.Type.Linear,
                                            bool scaled = true, bool accountForPitch = false) =>
            TweenProperty(TweenType.Volume, from, to, duration, easing, scaled, accountForPitch,
                          x => Source.volume = x, out routine);

        public AudioManagerItem TweenPitch(float from, float to,
                                           float duration, out Coroutine routine,
                                           Easings.Type easing = Easings.Type.Linear,
                                           bool scaled = true, bool accountForPitch = false) =>
            TweenProperty(TweenType.Pitch, from, to, duration, easing, scaled, accountForPitch,
                          x => Pitch = x, out routine);

        private AudioManagerItem TweenProperty(TweenType type, float from, float to,
                                               float duration, Easings.Type easing,
                                               bool scaled, bool scaleWithPitch,
                                               Action<float> callback, out Coroutine routine)
        {
            if (_tweenRoutines.TryGetValue(type, out routine) && routine != null)
                StopCoroutine(routine);
            _tweenRoutines[type] = routine = StartCoroutine(
                                       TweenProperty(type, from, to, duration, easing,
                                                     scaled, scaleWithPitch, callback));
            return this;
        }

        private IEnumerator TweenProperty(TweenType type, float from, float to,
                                          float duration, Easings.Type easing,
                                          bool scaled, bool scaleWithPitch, Action<float> callback)
        {
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                // Using unscaled pitch intentionally as this is alr scaled if the source is scaled
                var t = Utils.Tween(ref lerpPos, duration, easing, scaled,
                                    scaleWithPitch ? Pitch : 1);
                callback(Mathf.Lerp(from, to, t));
                yield return null;
            }

            _tweenRoutines[type] = null;
        }
    }
}