using System;
using UnityEngine;

namespace AudioManager.Effects
{
    [Serializable]
    public class Echo : AudioEffect
    {
        [Range(10, 5000)]
        public float Delay = 500;

        [Range(0, 1)]
        public float DecayRatio = 0.5f;

        [Range(0, 1)]
        public float DryMix = 1;

        [Range(0, 1)]
        public float WetMix = 1;

        public override void ApplyEffect(AudioManagerItem item)
        {
            if (!Enabled)
            {
                ClearEffect(item);
                return;
            }

            var filter = item.gameObject.GetOrAddComponent<AudioEchoFilter>();
            item.EffectsByType[typeof(AudioEchoFilter)].Add(this);

            filter.delay = Delay;
            filter.decayRatio = DecayRatio;
            filter.dryMix = DryMix;
            filter.wetMix = WetMix;
        }

        public override void ClearEffect(AudioManagerItem item) =>
            item.EffectsByType[typeof(AudioEchoFilter)].Remove(this);
    }
}