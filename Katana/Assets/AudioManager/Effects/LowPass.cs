using System;
using UnityEngine;

namespace AudioManager.Effects
{
    [Serializable]
    public class LowPass : AudioEffect
    {
        [Range(10, 22000)]
        public float LowPassCutoffFrequency = 5007.7f;

        [Range(1, 10)]
        public float LowPassResonanceQ = 1;

        public override Type GetEffectType() => typeof(AudioLowPassFilter);

        public override void ApplyEffect(AudioManagerItem item)
        {
            var filter = item.gameObject.GetOrAddComponent<AudioLowPassFilter>();
            filter.cutoffFrequency = LowPassCutoffFrequency;
            filter.lowpassResonanceQ = LowPassResonanceQ;
        }
    }
}