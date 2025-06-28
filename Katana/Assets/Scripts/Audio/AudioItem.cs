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
        [ValidateInput(nameof(ValidateResource))]
        [EnableIf(nameof(ResourceAssignmentType), ResourceAssignmentType.Resource)]
        public AudioResource Resource;
        private bool ValidateResource =>
            ResourceAssignmentType != ResourceAssignmentType.Resource || Resource;

        [HideLabel]
        [ValidateInput(nameof(ValidateResourceName), "Resource Name can't be empty")]
        [EnableIf(nameof(ResourceAssignmentType), ResourceAssignmentType.Name)]
        public string ResourceName;
        private bool ValidateResourceName => !string.IsNullOrEmpty(ResourceName);


        [Title("Mixer Group")]
        [HideLabel]
        public AudioMixerGroup MixerGroup;

        [Title("Source")]
        [HideLabel]
        [EnumToggle]
        public SourceType SourceType;

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

        [EnableIf(nameof(SourceType), SourceType.Attached)]
        public bool AssignTargetAtRuntime;

        // TODO: Replace with a single EnableIf when implemented
        [HideLabel]
        [ValidateInput(nameof(ValidateAttachTarget))]
        [EnableIf(nameof(SourceType), SourceType.Attached)]
        [EnableIf(nameof(AssignTargetAtRuntime), false)]
        public GameObject AttachTarget;
        private bool ValidateAttachTarget =>
            SourceType != SourceType.Attached || AssignTargetAtRuntime || AttachTarget;

        [Title("Settings")]
        public bool UseSettingsPreset;

        [EnableIf(nameof(UseSettingsPreset), false)]
        public AudioItemSettings Settings;

        [ValidateInput(nameof(ValidateSettingsPreset))]
        [EnableIf(nameof(UseSettingsPreset), true)]
        [PreviewScriptable]
        public AudioItemSettingsPreset SettingsPreset;
        private bool ValidateSettingsPreset => !UseSettingsPreset || SettingsPreset;
        
        public string Name =>
            ResourceAssignmentType switch
            {
                ResourceAssignmentType.ResourceItem => AudioResourceItem.Name,
                ResourceAssignmentType.Resource => Resource.name,
                ResourceAssignmentType.Name => ResourceName,
                _ => throw new ArgumentOutOfRangeException(nameof(ResourceAssignmentType))
            };
    }
}