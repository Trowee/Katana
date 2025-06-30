using System;
using UnityEngine;

namespace Assets.Scripts.Audio.Effects
{
    [Serializable]
    public class Distortion : AudioEffect
    {
        [Range(0, 1)]
        public float Level = 0.5f;

        public override void ApplyEffect(AudioManagerItem item)
        {
            if (!Enabled)
            {
                ClearEffect(item);
                return;
            }

            var filter = item.gameObject.GetComponent<AudioDistortionFilter>();
            if (filter == null)
            {
                filter = item.gameObject.AddComponent<AudioDistortionFilter>();
                item.EffectCounts[typeof(AudioDistortionFilter)].Add(this);
            }

            filter.distortionLevel = Level;
        }

        public override void ClearEffect(AudioManagerItem item)
        {
            item.EffectCounts[typeof(AudioDistortionFilter)].Remove(this);
        }
    }
}