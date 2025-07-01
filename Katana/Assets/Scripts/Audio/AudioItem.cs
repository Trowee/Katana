using System;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Audio.Effects;
using NnUtils.Modules.Easings;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Audio
{
    // TODO: Replace ValidateInput with Required when fixed for EnableIf
    [Serializable]
    public class AudioItem
    {
        [Title("Resource Assignment Type")]
        [HideLabel]
        [EnumToggle]
        public ResourceAssignmentType ResourceAssignmentType;

        [HideLabel]
        [ValidateInput(nameof(ValidateResourceItem))]
        [EnableIf(nameof(ResourceAssignmentType), ResourceAssignmentType.ResourceItem)]
        [PreviewScriptable]
        public AudioResourceItem AudioResourceItem;

        [HideLabel]
        [ValidateInput(nameof(ValidateAudioResource))]
        [EnableIf(nameof(ResourceAssignmentType), ResourceAssignmentType.Resource)]
        public AudioResource AudioResource;

        [HideLabel]
        [ValidateInput(nameof(ValidateResourceName), "Audio Resource Name can't be empty")]
        [EnableIf(nameof(ResourceAssignmentType), ResourceAssignmentType.Name)]
        public string AudioResourceName;

        [HideInInspector]
        public bool OverrideMixerGroup;

        [Title("Mixer Group")]
        [HideLabel]
        [Optional(nameof(OverrideMixerGroup), "")]
        public AudioMixerGroup MixerGroup;

        [Title("Source")]
        [HideLabel]
        [EnumToggle]
        public SourceType SourceType;

        [HorizontalGroup("Play")]
        [Title("Reuse Source")]
        [HideLabel]
        public bool ReuseSource;

        [HideInInspector]
        public bool OverridePlayOnAwake;

        [HorizontalGroup("Play")]
        [Title("Play On Awake")]
        [HideLabel]
        [Optional(nameof(OverridePlayOnAwake), "")]
        public bool PlayOnAwake;

        [HideInInspector]
        public bool OverrideScaled;

        [HorizontalGroup("Play")]
        [Title("Scaled")]
        [HideLabel]
        [Optional(nameof(OverrideScaled), "")]
        public bool Scaled;

        [HorizontalGroup("Destroy")]
        [Title("Destroy Source On Finished")]
        [HideLabel]
        public bool DestroySourceOnFinished;

        [HorizontalGroup("Destroy")]
        [Title("Destroy Target On Finished")]
        [HideLabel]
        public bool DestroyTargetOnFinished;

        [HideLabel]
        [EnableIf(nameof(SourceType), SourceType.Positional)]
        public Vector3 Position;

        [EnableIf(nameof(SourceType), SourceType.Object)]
        public bool AssignTargetAtRuntime;

        // TODO: Replace with a single EnableIf when implemented
        [HideLabel]
        [ValidateInput(nameof(ValidateAttachTarget))]
        [EnableIf(nameof(SourceType), SourceType.Object)]
        [EnableIf(nameof(AssignTargetAtRuntime), false)]
        public GameObject Target;
        
        [HideInInspector]
        public bool OverrideFadeIn;
        
        [TabGroup("Fade In")]
        [Optional(nameof(OverrideFadeIn))]
        public bool FadeIn;

        [TabGroup("Fade In")]
        [Optional(nameof(OverrideFadeIn), displayCheckbox: false)]
        public float FadeInTime;

        [TabGroup("Fade In")]
        [Optional(nameof(OverrideFadeIn), displayCheckbox: false)]
        public Easings.Type FadeInEasing;

        [HideInInspector]
        public bool OverrideFadeOut;
        
        [TabGroup("Fade Out")]
        [Optional(nameof(OverrideFadeOut))]
        public bool FadeOut;

        [TabGroup("Fade Out")]
        [Optional(nameof(OverrideFadeOut), displayCheckbox: false)]
        public float FadeOutTime;

        [TabGroup("Fade Out")]
        [Optional(nameof(OverrideFadeOut), displayCheckbox: false)]
        public Easings.Type FadeOutEasing;

        [Title("Settings")]
        public bool OverrideSettings;

        [Optional(nameof(OverrideSettings), displayCheckbox: false)]
        public bool ReloadSettingsEveryPlay;

        [Optional(nameof(OverrideSettings), displayCheckbox: false)]
        public bool UseSettingsPreset;

        [EnableIf(nameof(UseSettingsPreset), false)]
        // TODO: Report to artifice, it doesn't work under optional
        //[Optional(nameof(OverrideSettings), displayCheckbox: false)]
        public AudioSettings AudioSettings;

        [ValidateInput(nameof(ValidateSettingsPreset))]
        [EnableIf(nameof(UseSettingsPreset), true)]
        [Optional(nameof(OverrideSettings), displayCheckbox: false)]
        [PreviewScriptable]
        public AudioSettingsPreset AudioSettingsPreset;

        [Title("Effects")]
        public bool OverrideEffects;

        [Optional(nameof(OverrideEffects), displayCheckbox: false)]
        public bool UseEffectsPreset;

        [EnableIf(nameof(UseEffectsPreset), false)]
        // TODO: Report to artifice, it doesn't work under optional
        //[Optional(nameof(OverrideEffects), displayCheckbox: false)]
        public AudioEffects AudioEffects;

        [ValidateInput(nameof(ValidateEffectsPreset))]
        [EnableIf(nameof(UseEffectsPreset), true)]
        [Optional(nameof(OverrideEffects), displayCheckbox: false)]
        [PreviewScriptable]
        public AudioEffectsPreset AudioEffectsPreset;

        public AudioItem() : this(ResourceAssignmentType.ResourceItem)
        {
        }

        public AudioItem(AudioResourceItem resourceItem) : this(
            ResourceAssignmentType.ResourceItem,
            resourceItem,
            overrideMixerGroup: true,
            mixerGroup: resourceItem.MixerGroup,
            sourceType: SourceType.Manager,
            reuseSource: true,
            overridePlayOnAwake: true,
            playOnAwake: resourceItem.PlayOnAwake,
            overrideScaled: true,
            scaled: resourceItem.Scaled,
            position: resourceItem.Position,
            overrideFadeIn: true,
            fadeIn: resourceItem.FadeIn,
            fadeInTime: resourceItem.FadeInTime,
            fadeInEasing: resourceItem.FadeInEasing,
            overrideFadeOut: true,
            fadeOut: resourceItem.FadeOut,
            fadeOutTime: resourceItem.FadeOutTime,
            fadeOutEasing: resourceItem.FadeOutEasing,
            overrideSettings: true,
            reloadSettingsEveryPlay: resourceItem.ReloadSettingsEveryPlay,
            useSettingsPreset: resourceItem.UseSettingsPreset,
            settings: resourceItem.Settings,
            audioSettingsPreset: resourceItem.SettingsPreset,
            overrideEffects: true,
            useEffectsPreset: resourceItem.UseEffectsPreset,
            audioEffects: resourceItem.AudioEffects,
            audioEffectsPreset: resourceItem.AudioEffectsPreset)
        {
        }

        public AudioItem(ResourceAssignmentType resourceAssignmentType = default,
                         AudioResourceItem audioResourceItem = null,
                         AudioResource audioResource = null,
                         string audioResourceName = null,
                         bool overrideMixerGroup = false,
                         AudioMixerGroup mixerGroup = null,
                         SourceType sourceType = default,
                         bool reuseSource = true,
                         bool overridePlayOnAwake = false,
                         bool playOnAwake = false,
                         bool overrideScaled = false,
                         bool scaled = true,
                         bool destroySourceOnFinished = false,
                         bool destroyTargetOnFinished = false,
                         Vector3 position = default,
                         bool assignTargetAtRuntime = false,
                         GameObject target = null,
                         bool overrideFadeIn = false,
                         bool fadeIn = false,
                         float fadeInTime = 0,
                         Easings.Type fadeInEasing = Easings.Type.Linear,
                         bool overrideFadeOut = false,
                         bool fadeOut = false,
                         float fadeOutTime = 0,
                         Easings.Type fadeOutEasing = Easings.Type.Linear,
                         bool reloadSettingsEveryPlay = true,
                         bool overrideSettings = false,
                         bool useSettingsPreset = false,
                         AudioSettings settings = null,
                         AudioSettingsPreset audioSettingsPreset = null,
                         bool overrideEffects = false,
                         bool useEffectsPreset = false,
                         AudioEffects audioEffects = null,
                         AudioEffectsPreset audioEffectsPreset = null)
        {
            ReuseSource = reuseSource;
            ResourceAssignmentType = resourceAssignmentType;
            AudioResourceItem = audioResourceItem;
            AudioResource = audioResource;
            AudioResourceName = audioResourceName;
            OverrideMixerGroup = overrideMixerGroup;
            MixerGroup = mixerGroup;
            SourceType = sourceType;
            OverridePlayOnAwake = overridePlayOnAwake;
            PlayOnAwake = playOnAwake;
            OverrideScaled = overrideScaled;
            Scaled = scaled;
            DestroySourceOnFinished = destroySourceOnFinished;
            DestroyTargetOnFinished = destroyTargetOnFinished;
            Position = position;
            AssignTargetAtRuntime = assignTargetAtRuntime;
            Target = target;
            OverrideFadeIn = overrideFadeIn;
            FadeIn = fadeIn;
            FadeInTime = fadeInTime;
            FadeInEasing = fadeInEasing;
            OverrideFadeOut = overrideFadeOut;
            FadeOut = fadeOut;
            FadeOutTime = fadeOutTime;
            FadeOutEasing = fadeOutEasing;
            OverrideSettings = overrideSettings;
            ReloadSettingsEveryPlay = reloadSettingsEveryPlay;
            UseSettingsPreset = useSettingsPreset;
            AudioSettings = settings ?? new();
            AudioSettingsPreset = audioSettingsPreset;
            OverrideEffects = overrideEffects;
            UseEffectsPreset = useEffectsPreset;
            AudioEffects = audioEffects ?? new();
            AudioEffectsPreset = audioEffectsPreset;
        }

        private bool ValidateResourceItem =>
            ResourceAssignmentType != ResourceAssignmentType.ResourceItem || AudioResourceItem;
        private bool ValidateAudioResource =>
            ResourceAssignmentType != ResourceAssignmentType.Resource || AudioResource;
        private bool ValidateResourceName => !string.IsNullOrEmpty(AudioResourceName);
        private bool ValidateAttachTarget =>
            SourceType != SourceType.Object || AssignTargetAtRuntime || Target;
        private bool ValidateSettingsPreset => !UseSettingsPreset || AudioSettingsPreset;

        public AudioSettings Settings =>
            UseSettingsPreset ? AudioSettingsPreset.Settings : AudioSettings;
        private bool ValidateEffectsPreset => !UseEffectsPreset || AudioEffectsPreset;

        public AudioEffects Effects => UseEffectsPreset ? AudioEffectsPreset.Effects : AudioEffects;

        public string Name =>
            ResourceAssignmentType switch
            {
                ResourceAssignmentType.ResourceItem => AudioResourceItem.Name,
                ResourceAssignmentType.Resource => AudioResource.name,
                ResourceAssignmentType.Name => AudioResourceName,
                _ => throw new ArgumentOutOfRangeException(nameof(ResourceAssignmentType))
            };

        public AudioResource GetAudioResource(AudioSource source) =>
            ResourceAssignmentType switch
            {
                ResourceAssignmentType.ResourceItem => AudioResourceItem.Resource,
                ResourceAssignmentType.Resource => AudioResource,
                ResourceAssignmentType.Name => source.resource,
                _ => throw new(
                         "(Audio Item) ResourceAssignmentType must not be set to 'Manual' at the time of calling the GetAudioResource function")
            };

        public AudioSource ApplySettingsToSource(AudioSource source)
        {
            if (OverridePlayOnAwake) source.playOnAwake = PlayOnAwake;
            if (OverrideMixerGroup) source.outputAudioMixerGroup = MixerGroup;
            source.resource = GetAudioResource(source);
            return !OverrideSettings ? source : Settings.ApplyToSource(source);
        }
    }
}