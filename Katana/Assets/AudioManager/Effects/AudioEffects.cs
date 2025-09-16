using System;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using UnityEngine;

namespace AudioManager.Effects
{
    [Serializable]
    public class AudioEffects
    {
        [ValidateInput(nameof(ValidateEffects))]
        [SerializeReference, ForceArtifice]
        public List<AudioEffect> Effects;

        private bool ValidateEffects(ref string msg)
        {
            Dictionary<Type, int> existingEffectTypes = new();
            msg = string.Empty;

            for (int i = 0; i < Effects.Count; i++)
            {
                var effect = Effects[i];

                if (effect == null)
                {
                    msg += $"Effect at Index {i} can't be null\n";
                    continue;
                }

                var effectType = effect.GetEffectType();
                if (existingEffectTypes.TryGetValue(effectType, out var existingIndex))
                {
                    msg += $"Duplicate {effect} at Indices {existingIndex} and {i}\n";
                    continue;
                }

                existingEffectTypes.Add(effectType, i);
            }

            if (!string.IsNullOrEmpty(msg))
            {
                msg = msg.TrimEnd();
                return false;
            }

            return true;
        }

        public void ApplyEffects(AudioManagerItem item) =>
            Effects.ForEach(x => x.ApplyEffect(item));

        public void DestroyEffects(AudioManagerItem item) =>
            Effects.ForEach(x => UnityEngine.Object.Destroy(item.GetComponent(x.GetEffectType())));
    }
}