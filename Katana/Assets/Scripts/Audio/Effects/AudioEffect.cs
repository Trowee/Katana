using System;
using UnityEngine;

namespace Assets.Scripts.Audio.Effects
{
    [Serializable]
    public abstract class AudioEffect
    {
        public bool Enabled;
        public abstract void ApplyEffect(AudioManagerItem item);
    }
}