using System;
using NnUtils.Scripts;
using UnityEngine;

namespace Assets.Scripts.Audio.Effects
{
    [Serializable]
    public class LowPass : AudioEffect
    {
        [Range(10, 22000)]
        public float LowPassCutoffFrequency = 5007.7f;

        [Range(1, 10)]
        public float LowPassResonanceQ = 1;

        public override void ApplyEffect(AudioManagerItem item)
        {
            if (!Enabled)
            {
                ClearEffect(item);
                return;
            }

            var filter = item.gameObject.GetOrAddComponent<AudioLowPassFilter>();
            item.EffectCounts[typeof(AudioLowPassFilter)].Add(this);

            filter.cutoffFrequency = LowPassCutoffFrequency;
            filter.lowpassResonanceQ = LowPassResonanceQ;
        }

        public override void ClearEffect(AudioManagerItem item) =>
            item.EffectCounts[typeof(AudioLowPassFilter)].Remove(this);
    }
}