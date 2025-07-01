using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Audio.Effects;
using NnUtils.Modules.Easings;
using NnUtils.Scripts;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioManagerItem : MonoBehaviour
    {
        /// AudioItem this Item was created from
        public AudioItem OriginalAudioItem;

        /// Current AudioItem
        public AudioItem AudioItem;

        /// Audio Source
        public AudioSource Source;

        /// Whether Source is paused
        public bool Paused;

        /// Unscaled Pitch
        public float Pitch = 1;

        public enum TweenType
        {
            Volume,
            Pitch
        }

        #region Audio Item Property Getters

        private T GetOverrideValue<T>(
            bool audioOverride, T audioValue,
            bool originalOverride, T originalValue,
            T defaultValue) =>
            audioOverride ? audioValue : originalOverride ? originalValue : defaultValue;

        public bool PlayOnAwake => GetOverrideValue(
            AudioItem.OverridePlayOnAwake, AudioItem.PlayOnAwake,
            OriginalAudioItem.OverridePlayOnAwake, OriginalAudioItem.PlayOnAwake, false);

        public bool Scaled => GetOverrideValue(
            AudioItem.OverrideScaled, AudioItem.Scaled,
            OriginalAudioItem.OverrideScaled, OriginalAudioItem.Scaled, false);

        public bool FadeIn => GetOverrideValue(
            AudioItem.OverrideFadeIn, AudioItem.FadeIn,
            OriginalAudioItem.OverrideFadeIn, OriginalAudioItem.FadeIn, false);
        public float FadeInTime => GetOverrideValue(
            AudioItem.OverrideFadeIn, AudioItem.FadeInTime,
            OriginalAudioItem.OverrideFadeIn, OriginalAudioItem.FadeInTime, 0);
        public Easings.Type FadeInEasing => GetOverrideValue(
            AudioItem.OverrideFadeIn, AudioItem.FadeInEasing,
            OriginalAudioItem.OverrideFadeIn, OriginalAudioItem.FadeInEasing, Easings.Type.Linear);

        public bool FadeOut => GetOverrideValue(
            AudioItem.OverrideFadeOut, AudioItem.FadeOut,
            OriginalAudioItem.OverrideFadeOut, OriginalAudioItem.FadeOut, false);
        public float FadeOutTime => GetOverrideValue(
            AudioItem.OverrideFadeOut, AudioItem.FadeOutTime,
            OriginalAudioItem.OverrideFadeOut, OriginalAudioItem.FadeOutTime, 0);
        public Easings.Type FadeOutEasing => GetOverrideValue(
            AudioItem.OverrideFadeOut, AudioItem.FadeOutEasing,
            OriginalAudioItem.OverrideFadeOut, OriginalAudioItem.FadeOutEasing, Easings.Type.Linear);
        
        #endregion
        
        public readonly Dictionary<Type, HashSet<AudioEffect>> EffectCounts = new()
        {
            { typeof(AudioChorusFilter), new() },
            { typeof(AudioDistortionFilter), new() },
            { typeof(AudioEchoFilter), new() },
            { typeof(AudioHighPassFilter), new() },
            { typeof(AudioLowPassFilter), new() },
            { typeof(AudioReverbFilter), new() }
        };

        private void Update()
        {
            // Update Source Pitch
            if (Source.isPlaying) Source.pitch = Scaled ? Pitch * Time.timeScale: Pitch;
        }

        public AudioManagerItem ApplySettings(bool initial = false)
        {
            OriginalAudioItem.ApplySettingsToSource(Source);
            if (AudioItem.OverrideSettings)
                if (initial || AudioItem.ReloadSettingsEveryPlay)
                    AudioItem.ApplySettingsToSource(Source);
            
            // Update pitch once before playing
            Pitch = Source.pitch;
            Source.pitch = Scaled ? Pitch * Time.timeScale : Pitch;
            
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

            foreach (var ec in EffectCounts)
                if (EffectCounts[ec.Key].Count < 1 &&
                    gameObject.TryGetComponent(ec.Key, out var effect))
                    DestroyImmediate(effect);

            return this;
        }

        public AudioManagerItem Play()
        {
            if (Paused) return Resume();
            Stop();
            _playRoutine = StartCoroutine(PlayRoutine());
            return this;
        }

        public AudioManagerItem Stop()
        {
            if (_playRoutine != null)
            {
                Source.Stop();
                StopCoroutine(_playRoutine);
                _playRoutine = null;
                Paused = false;
            }
            foreach (var routine in _tweenRoutines) StopCoroutine(_tweenRoutines[routine.Key]);
            
            if (AudioItem.DestroyTargetOnFinished) DestroyImmediate(gameObject);
            else if (AudioItem.DestroySourceOnFinished) DestroyImmediate(Source);
            return this;
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

        private Coroutine _playRoutine;
        private IEnumerator PlayRoutine()
        {
            if (FadeIn) TweenVolume(0, Source.volume, FadeInTime, out _, FadeInEasing, Scaled);
            Source.Play();
            
            if (FadeOut)
            {
                var fadeOutSample = Source.clip.samples -
                                    Mathf.RoundToInt(Source.clip.frequency * (FadeOutTime + 1.05f));
                
                yield return new WaitUntil(() => Source.timeSamples >= fadeOutSample);
                yield return TweenVolume(Source.volume, 0, FadeOutTime, out _, FadeOutEasing);
            }
            
            yield return new WaitWhile(() => Source.isPlaying);
            Stop();
        }
        
        public AudioManagerItem TweenVolume(float from, float to,
                                            float duration, out Coroutine routine,
                                            Easings.Type easing = Easings.Type.Linear,
                                            bool scaled = true) =>
            TweenProperty(TweenType.Volume, from, to, duration, easing, scaled,
                          x => Source.volume = x, out routine);

        public AudioManagerItem TweenPitch(float from, float to,
                                           float duration, out Coroutine routine,
                                           Easings.Type easing = Easings.Type.Linear,
                                           bool scaled = true) =>
            TweenProperty(TweenType.Pitch, from, to, duration, easing, scaled,
                          x => Pitch = x, out routine);

        private readonly Dictionary<TweenType, Coroutine> _tweenRoutines = new();
        
        private AudioManagerItem TweenProperty(TweenType type, float from, float to,
                                               float duration, Easings.Type easing, bool scaled,
                                               Action<float> callback, out Coroutine routine)
        {
            routine = _tweenRoutines[type];
            if (routine != null) StopCoroutine(routine);
            _tweenRoutines[type] = routine = StartCoroutine(
                                 TweenProperty(type, from, to, duration, easing, scaled, callback));
            return this;
        }

        private IEnumerator TweenProperty(TweenType type, float from, float to,
                                          float duration, Easings.Type easing,
                                          bool scaled, Action<float> callback)
        {
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = Misc.Tween(ref lerpPos, duration, easing, !scaled);
                callback(Mathf.Lerp(from, to, t));
                yield return null;
            }

            _tweenRoutines[type] = null;
        }
    }
}