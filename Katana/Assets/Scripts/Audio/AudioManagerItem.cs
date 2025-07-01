using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Audio.Effects;
using NnUtils.Modules.Easings;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioManagerItem : MonoBehaviour
    {
        public enum TweenType
        {
            Volume,
            Pitch
        }
        
        /// AudioItem this Item was created from
        public AudioItem OriginalAudioItem;

        /// Current AudioItem
        public AudioItem AudioItem;

        /// Audio Source
        public AudioSource Source;

        /// Whether Source is paused
        public bool Paused { get; private set; }

        /// Unscaled Pitch
        public float Pitch { get; private set; } = 1;

        public readonly Dictionary<Type, HashSet<AudioEffect>> EffectCounts = new()
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
            // Update Source Pitch
            if (Source.isPlaying) Source.pitch = this.Scaled() ? Pitch * Time.timeScale: Pitch;
        }

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
            
            foreach (var routine in _tweenRoutines)
                if (routine.Value != null) StopCoroutine(routine.Value);
            
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
            var scaled = this.Scaled();
            var fadeInTime = this.FadeInTime();
            var fadeInEasing = this.FadeInEasing();

            if (this.FadeIn())
                TweenVolume(0, Source.volume, fadeInTime, out _, fadeInEasing, scaled, true);
            Source.Play();
            
            if (this.FadeOut())
            {
                var fadeOutTime = this.FadeOutTime();
                var fadeOutEasing = this.FadeOutEasing();

                int fadeOutSample() =>
                    Source.clip.samples - Mathf.RoundToInt(Source.clip.frequency *
                                                           (fadeOutTime / Source.pitch + 0.05f));

                yield return new WaitUntil(() => Source.timeSamples >= fadeOutSample());
                yield return TweenVolume(Source.volume, 0, fadeOutTime, out _,
                                         fadeOutEasing, scaled, true);
            }
            
            yield return new WaitWhile(() => Source.isPlaying);
            Stop();
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
                                               bool scaled, bool accountForPitch,
                                               Action<float> callback, out Coroutine routine)
        {
            if (_tweenRoutines.TryGetValue(type, out routine) && routine != null)
                StopCoroutine(routine);
            _tweenRoutines[type] = routine = StartCoroutine(
                                       TweenProperty(type, from, to, duration, easing,
                                                     scaled, accountForPitch, callback));
            return this;
        }

        private IEnumerator TweenProperty(TweenType type, float from, float to,
                                          float duration, Easings.Type easing,
                                          bool scaled, bool accountForPitch, Action<float> callback)
        {
            float totalTime = 0;
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                // Using unscaled pitch intentionally as this is alr scaled if the source is scaled
                totalTime += Time.deltaTime;
                var t = Utils.Tween(ref lerpPos, duration, easing, !scaled,
                                    accountForPitch ? Pitch : 1);
                callback(Mathf.Lerp(from, to, t));
                yield return null;
            }

            Debug.Log(totalTime);
            _tweenRoutines[type] = null;
        }
    }
}