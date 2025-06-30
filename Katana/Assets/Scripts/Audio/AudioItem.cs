using System;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Audio.Effects;
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
        private bool ValidateResourceItem =>
            ResourceAssignmentType != ResourceAssignmentType.ResourceItem || AudioResourceItem;

        [HideLabel]
        [ValidateInput(nameof(ValidateAudioResource))]
        [EnableIf(nameof(ResourceAssignmentType), ResourceAssignmentType.Resource)]
        public AudioResource AudioResource;
        private bool ValidateAudioResource =>
            ResourceAssignmentType != ResourceAssignmentType.Resource || AudioResource;

        [HideLabel]
        [ValidateInput(nameof(ValidateResourceName), "Audio Resource Name can't be empty")]
        [EnableIf(nameof(ResourceAssignmentType), ResourceAssignmentType.Name)]
        public string AudioResourceName;
        private bool ValidateResourceName => !string.IsNullOrEmpty(AudioResourceName);

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
        private bool ValidateAttachTarget =>
            SourceType != SourceType.Object || AssignTargetAtRuntime || Target;

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
        private bool ValidateSettingsPreset => !UseSettingsPreset || AudioSettingsPreset;

        public AudioSettings Settings =>
            UseSettingsPreset ? AudioSettingsPreset.Settings : AudioSettings;

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
        private bool ValidateEffectsPreset => !UseEffectsPreset || AudioEffectsPreset;

        public AudioEffects Effects => UseEffectsPreset ? AudioEffectsPreset.Effects : AudioEffects;

        public AudioItem() : this(ResourceAssignmentType.ResourceItem)
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
                         bool destroySourceOnFinished = false,
                         bool destroyTargetOnFinished = false,
                         Vector3 position = default,
                         bool assignTargetAtRuntime = false,
                         GameObject target = null,
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
            DestroySourceOnFinished = destroySourceOnFinished;
            DestroyTargetOnFinished = destroyTargetOnFinished;
            Position = position;
            AssignTargetAtRuntime = assignTargetAtRuntime;
            Target = target;
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