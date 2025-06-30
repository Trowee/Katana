using System;
using ArtificeToolkit.Attributes;
using NnUtils.Modules.Easings;
using UnityEngine;
using Object = UnityEngine.Object;

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
        public Easings.Type FadeInEasing;

        [TabGroup("Fade Out")]
        public bool FadeOut;

        [TabGroup("Fade Out")]
        [Optional(nameof(FadeOut), displayCheckbox: false)]
        public float FadeOutTime;

        [TabGroup("Fade Out")]
        [Optional(nameof(FadeOut), displayCheckbox: false)]
        public Easings.Type FadeOutEasing;

        [FoldoutGroup("Chorus")]
        public bool Chorus;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [UnityEngine.Range(0, 1)]
        public float ChorusDryMix;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [UnityEngine.Range(0, 1)]
        public float ChorusWetMix1;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [UnityEngine.Range(0, 1)]
        public float ChorusWetMix2;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [UnityEngine.Range(0, 1)]
        public float ChorusWetMix3;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [UnityEngine.Range(0.1f, 100)]
        public float ChorusDelay;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [UnityEngine.Range(0, 20)]
        public float ChorusRate;

        [FoldoutGroup("Chorus")]
        [EnableIf(nameof(Chorus), true)]
        [UnityEngine.Range(0, 1)]
        public float ChorusDepth;

        [FoldoutGroup("Distortion")]
        public bool Distortion;

        [FoldoutGroup("Distortion")]
        [EnableIf(nameof(Distortion), true)]
        [UnityEngine.Range(0, 1)]
        public float DistortionLevel;

        [FoldoutGroup("Echo")]
        public bool Echo;

        [FoldoutGroup("Echo")]
        [EnableIf(nameof(Echo), true)]
        [UnityEngine.Range(10, 5000)]
        public float EchoDelay;

        [FoldoutGroup("Echo")]
        [EnableIf(nameof(Echo), true)]
        [UnityEngine.Range(0, 1)]
        public float EchoDecayRatio;

        [FoldoutGroup("Echo")]
        [EnableIf(nameof(Echo), true)]
        [UnityEngine.Range(0, 1)]
        public float EchoDryMix;

        [FoldoutGroup("Echo")]
        [EnableIf(nameof(Echo), true)]
        [UnityEngine.Range(0, 1)]
        public float EchoWetMix;

        [FoldoutGroup("Highpass")]
        public bool Highpass;

        [FoldoutGroup("Highpass")]
        [EnableIf(nameof(Highpass), true)]
        [UnityEngine.Range(10, 22000)]
        public float HighpassCutoffFrequency;

        [FoldoutGroup("Highpass")]
        [EnableIf(nameof(Highpass), true)]
        [UnityEngine.Range(1, 10)]
        public float HighpassResonanceQ;

        [FoldoutGroup("Lowpass")]
        public bool Lowpass;

        [FoldoutGroup("Lowpass")]
        [EnableIf(nameof(Lowpass), true)]
        [UnityEngine.Range(10, 22000)]
        public float LowpassCutoffFrequency;

        [FoldoutGroup("Lowpass")]
        [EnableIf(nameof(Lowpass), true)]
        [UnityEngine.Range(1, 10)]
        public float LowpassResonanceQ;

        public void ApplyEffects(AudioManagerItem item)
        {
            // TODO: Fade in and out
            ApplyEffect(item, Chorus, typeof(AudioChorusFilter));
            ApplyEffect(item, Distortion, typeof(AudioDistortionFilter));
            ApplyEffect(item, Echo, typeof(AudioEchoFilter));
        }

        private void ApplyEffect(AudioManagerItem item, bool useEffect, Type effectType)
        {
            var component = item.gameObject.GetComponent(effectType);

            if (useEffect)
            {
                if (component == null) component = item.gameObject.AddComponent(effectType);
                switch (component)
                {
                    case AudioChorusFilter filter:
                        ApplyEffectSettings(filter);
                        return;
                    case AudioDistortionFilter filter:
                        ApplyEffectSettings(filter);
                        return;
                    case AudioEchoFilter filter:
                        ApplyEffectSettings(filter);
                        return;
                    case AudioHighPassFilter filter:
                        ApplyEffectSettings(filter);
                        return;
                    case AudioLowPassFilter filter:
                        ApplyEffectSettings(filter);
                        return;
                    case AudioReverbFilter filter:
                        ApplyEffectSettings(filter);
                        return;
                }
            }
            else if (component != null) Object.DestroyImmediate(component);
        }

        private void ApplyEffectSettings(AudioChorusFilter chorus)
        {
            chorus.dryMix = ChorusDryMix;
            chorus.wetMix1 = ChorusWetMix1;
            chorus.wetMix2 = ChorusWetMix2;
            chorus.wetMix3 = ChorusWetMix3;
            chorus.delay = ChorusDelay;
            chorus.rate = ChorusRate;
            chorus.depth = ChorusDepth;
        }

        private void ApplyEffectSettings(AudioDistortionFilter distortion)
        {
            distortion.distortionLevel = DistortionLevel;
        }

        private void ApplyEffectSettings(AudioEchoFilter echo)
        {
            echo.delay = EchoDelay;
            echo.decayRatio = EchoDecayRatio;
            echo.dryMix = EchoDryMix;
            echo.wetMix = EchoWetMix;
        }

        private void ApplyEffectSettings(AudioHighPassFilter highpass)
        {
            highpass.cutoffFrequency = HighpassCutoffFrequency;
            highpass.highpassResonanceQ = HighpassResonanceQ;
        }

        private void ApplyEffectSettings(AudioLowPassFilter lowpass)
        {
            lowpass.cutoffFrequency = LowpassCutoffFrequency;
            lowpass.lowpassResonanceQ = LowpassResonanceQ;
        }

        private void ApplyEffectSettings(AudioReverbFilter reverb)
        {

        }
    }
}