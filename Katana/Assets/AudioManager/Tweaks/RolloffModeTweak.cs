using System;
using ArtificeToolkit.Attributes;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class RolloffModeTweak : Tweak
    {
        [EnumToggle]
        public AudioRolloffMode RolloffMode;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.rolloffMode = RolloffMode;
            return source;
        }
    }
}