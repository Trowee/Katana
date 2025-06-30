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

        public AudioSource Source;
        private readonly Dictionary<int, Coroutine> _routines = new();

        public readonly Dictionary<Type, HashSet<AudioEffect>> EffectCounts = new()
        {
            { typeof(AudioChorusFilter), new() },
            { typeof(AudioDistortionFilter), new() },
            { typeof(AudioEchoFilter), new() },
            { typeof(AudioHighPassFilter), new() },
            { typeof(AudioLowPassFilter), new() },
            { typeof(AudioReverbFilter), new() }
        };

        public AudioManagerItem ApplySettings(bool initial = false)
        {
            OriginalAudioItem.ApplySettingsToSource(Source);
            if (AudioItem.OverrideSettings)
                if (initial || AudioItem.ReloadSettingsEveryPlay)
                    AudioItem.ApplySettingsToSource(Source);
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

        public AudioManagerItem TweenVolume(float from, float to,
                                            float duration, out Coroutine routine,
                                            Easings.Type easing = Easings.Type.Linear,
                                            bool scaled = true)
        {
            return TweenProperty(0, from, to, duration, easing, scaled,
                                 x => Source.volume = x, out routine);
        }

        public AudioManagerItem TweenPitch(float from, float to,
                                           float duration, out Coroutine routine,
                                           Easings.Type easing = Easings.Type.Linear,
                                           bool scaled = true)
        {
            return TweenProperty(0, from, to, duration, easing, scaled,
                                 x => Source.pitch = x, out routine);
        }

        private AudioManagerItem TweenProperty(int key, float from, float to,
                                               float duration, Easings.Type easing, bool scaled,
                                               Action<float> callback, out Coroutine routine)
        {
            if (_routines.TryGetValue(key, out routine)) StopCoroutine(routine);

            _routines[key] = routine = StartCoroutine(
                                 TweenProperty(key, from, to, duration, easing, scaled, callback));

            return this;
        }

        private IEnumerator TweenProperty(int key, float from, float to,
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

            _routines.Remove(key);
        }
    }
}