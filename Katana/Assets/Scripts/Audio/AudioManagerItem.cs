using System;
using System.Collections;
using System.Collections.Generic;
using NnUtils.Modules.Easings;
using NnUtils.Scripts;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioManagerItem : MonoBehaviour
    {
        private readonly Dictionary<int, Coroutine> _routines = new();
        
        /// AudioItem this Item was created from
        public AudioItem OriginalAudioItem;
        
        /// Current AudioItem
        public AudioItem AudioItem;
        
        public AudioSource Source;

        public AudioManagerItem ApplySettings(bool initial = false)
        {
            if (OriginalAudioItem.OverrideSettings)
                if (initial || OriginalAudioItem.ReloadSettingsEveryPlay)
                    AudioItem.Settings.ApplyToSource(Source);
            if (AudioItem.OverrideSettings)
                if (initial || AudioItem.ReloadSettingsEveryPlay)
                    AudioItem.Settings.ApplyToSource(Source);
            return this;
        }
        
        public AudioManagerItem ApplyEffects()
        {
            if (OriginalAudioItem.OverrideEffects)
                OriginalAudioItem.Effects.ApplyEffects(this);
            if (OriginalAudioItem != AudioItem && AudioItem.OverrideEffects)
                AudioItem.Effects.ApplyEffects(this);
            return this;
        }
        
        public AudioManagerItem TweenVolume(float from, float to,
                                            float duration, out Coroutine routine,
                                            Easings.Type easing = Easings.Type.Linear,
                                            bool scaled = true) =>
            TweenProperty(0, from, to, duration, easing, scaled,
                          x => Source.volume = x, out routine);

        public AudioManagerItem TweenPitch(float from, float to,
                                            float duration, out Coroutine routine,
                                            Easings.Type easing = Easings.Type.Linear,
                                            bool scaled = true) =>
            TweenProperty(0, from, to, duration, easing, scaled,
                          x => Source.pitch = x, out routine);

        private AudioManagerItem TweenProperty(int key, float from, float to,
                                                float duration, Easings.Type easing, bool scaled,
                                                Action<float> callback, out Coroutine routine)
        {
            if (_routines.TryGetValue(key, out routine))
            {
                StopCoroutine(routine);
            }

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