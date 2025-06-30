using System;
using ArtificeToolkit.Attributes;
using NnUtils.Modules.Easings;

namespace Assets.Scripts.Audio
{
    [Serializable]
    public class AudioEffects
    {
        [TabGroup("Fade In")]
        public bool FadeIn;
        
        [TabGroup("Fade In")]
        [Optional(nameof(FadeIn), displayCheckbox: false)]
        public float FadeInTime;
        
        [TabGroup("Fade In")]
        [Optional(nameof(FadeIn), displayCheckbox: false)]
        public Easings.Type  FadeInEasing;
        
        [TabGroup("Fade Out")]
        public bool FadeOut;
        
        [TabGroup("Fade Out")]
        [Optional(nameof(FadeOut), displayCheckbox: false)]
        public float FadeOutTime;
        
        [TabGroup("Fade Out")]
        [Optional(nameof(FadeOut), displayCheckbox: false)]
        public Easings.Type  FadeOutEasing;

        [FoldoutGroup("Chorus")]
        public bool Chorus;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [Range(0, 1)]
        public float ChorusDryMix;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [Range(0, 1)]
        public float ChorusWetMix1;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [Range(0, 1)]
        public float ChorusWetMix2;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [Range(0, 1)]
        public float ChorusWetMix3;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [Range(0.1f, 100)]
        public float ChorusDelay;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [Range(0, 20)]
        public float ChorusRate;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [Range(0, 1)]
        public float ChorusDepth;
    }
}