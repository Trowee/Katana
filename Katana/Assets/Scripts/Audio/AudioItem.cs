using System;
using ArtificeToolkit.Attributes;
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

        [Title("Mixer Group")]
        [HideLabel]
        public AudioMixerGroup MixerGroup;

        [Title("Source")]
        [HideLabel]
        [EnumToggle]
        public SourceType SourceType;

        [HorizontalGroup("Play")]
        [Title("Reuse Source")]
        [HideLabel]
        public bool ReuseSource;

        [HorizontalGroup("Play")]
        [Title("Play On Awake")]
        [HideLabel]
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
        public bool UseItemSettingsPreset;

        [EnableIf(nameof(UseItemSettingsPreset), false)]
        public AudioItemSettings ItemSettings;

        [ValidateInput(nameof(ValidateItemSettingsPreset))]
        [EnableIf(nameof(UseItemSettingsPreset), true)]
        [PreviewScriptable]
        public AudioItemSettingsPreset ItemSettingsPreset;
        private bool ValidateItemSettingsPreset => !UseItemSettingsPreset || ItemSettingsPreset;

        public AudioItem() : this(reuseSource: true) { }
        
        public AudioItem(ResourceAssignmentType resourceAssignmentType = default,
                         AudioResourceItem audioResourceItem = null,
                         AudioResource audioResource = null,
                         string audioResourceName = null,
                         AudioMixerGroup mixerGroup = null,
                         SourceType sourceType = default,
                         bool reuseSource = true,
                         bool playOnAwake = false,
                         bool destroySourceOnFinished = false,
                         bool destroyTargetOnFinished = false,
                         Vector3 position = default,
                         bool assignTargetAtRuntime = false,
                         GameObject target = null,
                         bool useItemSettingsPreset = false,
                         AudioItemSettings itemSettings = null,
                         AudioItemSettingsPreset itemSettingsPreset = null)
        {
            ReuseSource = reuseSource;
            ResourceAssignmentType = resourceAssignmentType;
            AudioResourceItem = audioResourceItem;
            AudioResource = audioResource;
            AudioResourceName = audioResourceName;
            MixerGroup = mixerGroup;
            SourceType = sourceType;
            PlayOnAwake = playOnAwake;
            DestroySourceOnFinished = destroySourceOnFinished;
            DestroyTargetOnFinished = destroyTargetOnFinished;
            Position = position;
            AssignTargetAtRuntime = assignTargetAtRuntime;
            Target = target;
            UseItemSettingsPreset = useItemSettingsPreset;
            ItemSettings = itemSettings ?? new();
            ItemSettingsPreset = itemSettingsPreset;
        }

        public string Name =>
            ResourceAssignmentType switch
            {
                ResourceAssignmentType.ResourceItem => AudioResourceItem.Name,
                ResourceAssignmentType.Resource => AudioResource.name,
                ResourceAssignmentType.Name => AudioResourceName,
                _ => throw new ArgumentOutOfRangeException(nameof(ResourceAssignmentType))
            };

        public AudioItemSettings Settings =>
            UseItemSettingsPreset ? ItemSettingsPreset.Settings : ItemSettings;

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
            source.playOnAwake = PlayOnAwake;
            source.outputAudioMixerGroup = MixerGroup;
            source.resource = GetAudioResource(source);
            return (UseItemSettingsPreset ? ItemSettingsPreset.Settings : ItemSettings)
                .ApplyToSource(source);
        }
    }
}