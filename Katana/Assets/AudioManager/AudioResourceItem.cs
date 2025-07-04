using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using AudioManager.Effects;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioManager
{
    [CreateAssetMenu(fileName = "ResourceItem",
                     menuName = "NnUtils/Audio Manager/Audio Resource Item")]
    public class AudioResourceItem : ScriptableObject
    {
        public string Name;
        public AudioResource Resource;

        [HorizontalGroup("Play")]
        [Title("Play On Awake")]
        [HideLabel]
        public bool PlayOnAwake;

        [HorizontalGroup("Play")]
        [Title("Loop")]
        [HideLabel]
        public bool Loop;

        [HorizontalGroup("Play")]
        [Title("Scaled")]
        [HideLabel]
        public bool Scaled = true;

        [Title("Source")]
        [ValidateInput(nameof(ValidateSourceType),
                       "SourceType can't be set to Object on an AudioResourceItem")]
        [HideLabel]
        [EnumToggle]
        public SourceType SourceType;

        [EnableIf(nameof(SourceType), SourceType.Positional)]
        public Vector3 Position;

        [TabGroup("Fade In")]
        public bool FadeIn;

        [TabGroup("Fade In")]
        [Optional(nameof(FadeIn), displayCheckbox: false)]
        public float FadeInTime;

        [TabGroup("Fade In")]
        [Optional(nameof(FadeIn), displayCheckbox: false)]
        public Easings.Type FadeInEasing;

        [TabGroup("Fade In")]
        public bool FadeInScale = true;

        [TabGroup("Fade In")]
        public bool FadeInScaleWithPitch = true;

        [TabGroup("Fade Out")]
        public bool FadeOut;

        [TabGroup("Fade Out")]
        [Optional(nameof(FadeOut), displayCheckbox: false)]
        public float FadeOutTime;

        [TabGroup("Fade Out")]
        [Optional(nameof(FadeOut), displayCheckbox: false)]
        public Easings.Type FadeOutEasing;

        [TabGroup("Fade Out")]
        public bool FadeOutScale = true;

        [TabGroup("Fade Out")]
        public bool FadeOutScaleWithPitch = true;

        [SerializeReference]
        [ForceArtifice]
        public List<IAppliable<AudioSource>> Tweaks;

        public bool UseEffectsPreset = false;

        [EnableIf(nameof(UseEffectsPreset), false)]
        public AudioEffects AudioEffects;

        [ValidateInput(nameof(ValidateEffectsPreset))]
        [EnableIf(nameof(UseEffectsPreset), true)]
        [PreviewScriptable]
        public AudioEffectsPreset AudioEffectsPreset;

        private bool ValidateSourceType => SourceType != SourceType.Object;
        private bool ValidateEffectsPreset => !UseEffectsPreset || AudioEffectsPreset;
    }
}