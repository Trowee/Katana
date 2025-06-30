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
            ClearEffect(item, typeof(AudioChorusFilter));
            ClearEffect(item, typeof(AudioDistortionFilter));
            ClearEffect(item, typeof(AudioEchoFilter));
            ClearEffect(item, typeof(AudioHighPassFilter));
            ClearEffect(item, typeof(AudioLowPassFilter));
            ClearEffect(item, typeof(AudioReverbFilter));
        }

        private void ClearEffect(AudioManagerItem item, Type effectType)
        {
            if (item.gameObject.TryGetComponent(effectType, out var effect))
                Object.DestroyImmediate(effect);
        }
    }
}