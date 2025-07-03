using System;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public abstract class Tweak
    {
        public abstract AudioSource Apply(AudioSource source);
    }
}