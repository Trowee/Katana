using System;
using ArtificeToolkit.Attributes;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class PriorityTweak : ITweak<AudioSource>
    {
        [Title("Priority")]
        [UnityEngine.Range(0, 256)]
        public int Priority = 128;
        
        public void Apply(AudioSource source) => source.priority = Priority;
    }
}