using System;
using ArtificeToolkit.Attributes;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class BypassTweak : ITweak<AudioSource>
    {
        [HorizontalGroup]
        [HideLabel]
        [Title("Bypass Effects")]
        public bool BypassEffects;

        [HorizontalGroup]
        [HideLabel]
        [EnableIf(nameof(BypassEffects), true)]
        [Title("Bypass Listener Effects")]
        public bool BypassListenerEffects;

        [HorizontalGroup]
        [HideLabel]
        [Title("Bypass Reverb Zones")]
        public bool BypassReverbZones;
        
        public void Apply(AudioSource target)
        {
            target.bypassEffects = BypassEffects;
            target.bypassListenerEffects = BypassListenerEffects;
            target.bypassReverbZones = BypassReverbZones;
        }
    }
}