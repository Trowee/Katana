using System;

namespace Assets.Scripts.Audio.Effects
{
    [Serializable]
    public abstract class AudioEffect
    {
        public bool Enabled;
        public abstract void ApplyEffect(AudioManagerItem item);
        public abstract void ClearEffect(AudioManagerItem item);
    }
}