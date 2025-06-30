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
        
        public AudioItem AudioItem;
        public AudioSource Source;
        
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