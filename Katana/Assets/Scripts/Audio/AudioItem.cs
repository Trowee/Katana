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
        [Title("Clip Assignment Type")]
        [HideLabel]
        [EnumToggle]
        public ClipAssignmentType ClipAssignmentType;

        [HideLabel]
        [ValidateInput(nameof(ValidateClipItem))]
        [EnableIf(nameof(ClipAssignmentType), ClipAssignmentType.ClipItem)]
        public AudioClipItem AudioClipItem;
        private bool ValidateClipItem =>
            ClipAssignmentType != ClipAssignmentType.ClipItem || AudioClipItem;

        [HideLabel]
        [ValidateInput(nameof(ValidateClip))]
        [EnableIf(nameof(ClipAssignmentType), ClipAssignmentType.Clip)]
        public AudioClip Clip;
        private bool ValidateClip =>
            ClipAssignmentType != ClipAssignmentType.Clip || Clip;

        [HideLabel]
        [ValidateInput(nameof(ValidateClipName), "Clip Name can't be empty")]
        [EnableIf(nameof(ClipAssignmentType), ClipAssignmentType.Name)]
        public string ClipName;
        private bool ValidateClipName => !string.IsNullOrEmpty(ClipName);


        [Title("Mixer Group")]
        [HideLabel]
        public AudioMixerGroup MixerGroup;

        [Title("Source Type")]
        [HideLabel]
        [EnumToggle]
        public SourceType SourceType;

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
    }
}