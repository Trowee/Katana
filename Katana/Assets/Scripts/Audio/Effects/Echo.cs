using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Audio.Effects
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
            var filter = item.gameObject.GetComponent<AudioEchoFilter>();
            if (!Enabled)
            {
                var count = item.EffectCounts[typeof(Echo)]--;
                if (count < 1 && filter != null) Object.DestroyImmediate(filter);
                return;
            }
            if (filter == null) filter = item.gameObject.AddComponent<AudioEchoFilter>();
            item.EffectCounts[typeof(Echo)]++;
            
            filter.delay = Delay;
            filter.decayRatio = DecayRatio;
            filter.dryMix = DryMix;
            filter.wetMix = WetMix;
        }
    }
}