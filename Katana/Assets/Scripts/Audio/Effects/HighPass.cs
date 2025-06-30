using System;
using UnityEngine;
using Object = UnityEngine.Object;

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
            var filter = item.gameObject.GetComponent<AudioHighPassFilter>();
            if (!Enabled)
            {
                if (filter != null) Object.DestroyImmediate(filter);
                return;
            }
            if (filter == null) filter = item.gameObject.AddComponent<AudioHighPassFilter>();
            
            filter.cutoffFrequency = HighPassCutoffFrequency;
            filter.highpassResonanceQ =  HighPassResonanceQ;
        }
    }
}