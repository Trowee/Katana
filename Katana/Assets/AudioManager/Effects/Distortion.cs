using System;
using UnityEngine;

namespace AudioManager.Effects
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

            var filter = item.gameObject.GetOrAddComponent<AudioDistortionFilter>();
            item.EffectsByType[typeof(AudioDistortionFilter)].Add(this);

            filter.distortionLevel = Level;
        }

        public override void ClearEffect(AudioManagerItem item) =>
            item.EffectsByType[typeof(AudioDistortionFilter)].Remove(this);
    }
}