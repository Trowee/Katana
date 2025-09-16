using System;

namespace AudioManager.Effects
{
    [Serializable]
    public abstract class AudioEffect
    {
        public abstract Type GetEffectType();
        public abstract void ApplyEffect(AudioManagerItem item);
    }
}