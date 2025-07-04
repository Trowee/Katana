using System;
using ArtificeToolkit.Attributes;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class RolloffModeTweak : IAppliable<AudioSource>
    {
        [EnumToggle]
        public AudioRolloffMode RolloffMode;
        
        public void Apply(AudioSource source) => source.rolloffMode = RolloffMode;
    }
}