using System;
using ArtificeToolkit.Attributes;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class BypassTweak : Tweak
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
        
        public override AudioSource Apply(AudioSource source)
        {
            source.bypassEffects = BypassEffects;
            source.bypassListenerEffects = BypassListenerEffects;
            source.bypassReverbZones = BypassReverbZones;
            return source;
        }
    }
}