using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Audio.Effects
{
    [Serializable]
    public class Distortion : AudioEffect
    {
        [Range(0, 1)]
        public float Level = 0.5f;
        
        public override void ApplyEffect(AudioManagerItem item)
        {
            var filter = item.gameObject.GetComponent<AudioDistortionFilter>();
            if (!Enabled)
            {
                var count = item.EffectCounts[typeof(Distortion)]--;
                if (count < 1 && filter != null) Object.DestroyImmediate(filter);
                return;
            }
            if (filter == null) filter = item.gameObject.AddComponent<AudioDistortionFilter>();
            item.EffectCounts[typeof(Distortion)]++;
            
            filter.distortionLevel = Level;
        }
    }
}