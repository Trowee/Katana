using System;
using NnUtils.Scripts;
using UnityEngine;

namespace Assets.Scripts.Audio.Effects
{
    [Serializable]
    public class HighPass : AudioEffect
    {
        [Range(10, 22000)]
        public float HighPassCutoffFrequency = 5000;

        [Range(1, 10)]
        public float HighPassResonanceQ = 1;

        public override void ApplyEffect(AudioManagerItem item)
        {
            if (!Enabled)
            {
                ClearEffect(item);
                return;
            }

            var filter = item.gameObject.GetOrAddComponent<AudioHighPassFilter>();
            item.EffectCounts[typeof(AudioHighPassFilter)].Add(this);

            filter.cutoffFrequency = HighPassCutoffFrequency;
            filter.highpassResonanceQ = HighPassResonanceQ;
        }

        public override void ClearEffect(AudioManagerItem item) =>
            item.EffectCounts[typeof(AudioHighPassFilter)].Remove(this);
    }
}