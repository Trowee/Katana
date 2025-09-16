using System;
using UnityEngine;

namespace AudioManager.Effects
{
    [Serializable]
    public class Distortion : AudioEffect
    {
        [Range(0, 1)]
        public float Level = 0.5f;

        public override Type GetEffectType() => typeof(AudioDistortionFilter);

        public override void ApplyEffect(AudioManagerItem item)
        {
            var filter = item.gameObject.GetOrAddComponent<AudioDistortionFilter>();
            filter.distortionLevel = Level;
        }
    }
}