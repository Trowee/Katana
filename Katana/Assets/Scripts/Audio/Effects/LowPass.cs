using System;
using UnityEngine;
using Object = UnityEngine.Object;

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
            var filter = item.gameObject.GetComponent<AudioLowPassFilter>();
            if (!Enabled)
            {
                if (filter != null) Object.DestroyImmediate(filter);
                return;
            }
            if (filter == null) filter = item.gameObject.AddComponent<AudioLowPassFilter>();
            
            filter.cutoffFrequency = LowPassCutoffFrequency;
            filter.lowpassResonanceQ =  LowPassResonanceQ;
        }
    }
}