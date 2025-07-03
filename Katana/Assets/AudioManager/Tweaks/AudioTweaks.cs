using System;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class AudioTweaks
    {
        [SerializeReference]
        [ForceArtifice]
        public List<Tweak> Tweaks;

        public AudioSource ApplyToSource(AudioSource source)
        {
            Tweaks.ForEach(setting => setting.Apply(source));
            return source;
        }
    }
}