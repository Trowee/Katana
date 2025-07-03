using System;
using ArtificeToolkit.Attributes;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class PriorityTweak : Tweak
    {
        [Title("Priority")]
        [UnityEngine.Range(0, 256)]
        public int Priority = 128;
        
        public override AudioSource Apply(AudioSource source)
        {
            source.priority = Priority;
            return source;
        }
    }
}