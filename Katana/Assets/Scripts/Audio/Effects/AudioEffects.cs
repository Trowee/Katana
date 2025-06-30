using System;
using ArtificeToolkit.Attributes;
using NnUtils.Modules.Easings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Audio.Effects
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
        public Easings.Type FadeInEasing;

        [TabGroup("Fade Out")]
        public bool FadeOut;

        [TabGroup("Fade Out")]
        [Optional(nameof(FadeOut), displayCheckbox: false)]
        public float FadeOutTime;

        [TabGroup("Fade Out")]
        [Optional(nameof(FadeOut), displayCheckbox: false)]
        public Easings.Type FadeOutEasing;

        public Chorus Chorus;

        public Distortion Distortion;

        public Echo Echo;

        public HighPass HighPass;
        
        public LowPass Lowpass;

        public void ApplyEffects(AudioManagerItem item)
        {
            // TODO: Fade in and out
            Chorus.ApplyEffect(item);
            Distortion.ApplyEffect(item);
            Echo.ApplyEffect(item);
            HighPass.ApplyEffect(item);
            Lowpass.ApplyEffect(item);
        }

        public void ClearEffects(AudioManagerItem item)
        {
            Chorus.ClearEffect(item);
            Distortion.ClearEffect(item);
            Echo.ClearEffect(item);
            HighPass.ClearEffect(item);
            Lowpass.ClearEffect(item);
        }
    }
}