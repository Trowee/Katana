using System;
using UnityEngine;

namespace Assets.Scripts.Audio.Effects
{
    [Serializable]
    public class Chorus : AudioEffect
    {
        [Range(0, 1)]
        public float DryMix = 0.5f;

        [Range(0, 1)]
        public float WetMix1 = 0.5f;

        [Range(0, 1)]
        public float WetMix2 = 0.5f;

        [Range(0, 1)]
        public float WetMix3 = 0.5f;

        [Range(0.1f, 100)]
        public float Delay = 40;

        [Range(0, 20)]
        public float Rate = 0.8f;

        [Range(0, 1)]
        public float Depth = 0.03f;

        public override void ApplyEffect(AudioManagerItem item)
        {
            if (!Enabled)
            {
                ClearEffect(item);
                return;
            }

            var filter = item.gameObject.GetComponent<AudioChorusFilter>();
            if (filter == null)
            {
                filter = item.gameObject.AddComponent<AudioChorusFilter>();
                item.EffectCounts[typeof(AudioChorusFilter)].Add(this);
            }

            filter.dryMix = DryMix;
            filter.wetMix1 = WetMix1;
            filter.wetMix2 = WetMix2;
            filter.wetMix3 = WetMix3;
            filter.delay = Delay;
            filter.rate = Rate;
            filter.depth = Depth;
        }

        public override void ClearEffect(AudioManagerItem item) =>
            item.EffectCounts[typeof(AudioChorusFilter)].Remove(this);
    }
}