using System;
using UnityEngine;

namespace AudioManager.Effects
{
    [Serializable]
    public class HighPass : AudioEffect
    {
        [Range(10, 22000)]
        public float HighPassCutoffFrequency = 5000;

        [Range(1, 10)]
        public float HighPassResonanceQ = 1;

        public override Type GetEffectType() => typeof(AudioHighPassFilter);

        public override void ApplyEffect(AudioManagerItem item)
        {
            var filter = item.gameObject.GetOrAddComponent<AudioHighPassFilter>();
            filter.cutoffFrequency = HighPassCutoffFrequency;
            filter.highpassResonanceQ = HighPassResonanceQ;
        }
    }
}