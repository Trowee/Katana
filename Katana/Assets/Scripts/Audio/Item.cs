using System;
using ArtificeToolkit.Attributes;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Audio
{
    // TODO: Replace ValidateInput with Required when fixed for EnableIf
    [Serializable]
    public class Item
    {
        [Title("Clip")]

        [EnumButtons]
        public ClipAssignmentType ClipAssignmentType;

        [ValidateInput(nameof(ValidateClipItem))]
        [EnableIf(nameof(ClipAssignmentType), ClipAssignmentType.ClipItem)]
        public ClipItem ClipItem;
        private bool ValidateClipItem =>
            ClipAssignmentType != ClipAssignmentType.ClipItem || ClipItem;

        [ValidateInput(nameof(ValidateClip))]
        [EnableIf(nameof(ClipAssignmentType), ClipAssignmentType.Clip)]
        public AudioClip Clip;
        private bool ValidateClip =>
            ClipAssignmentType != ClipAssignmentType.Clip || Clip;

        [ValidateInput(nameof(ValidateClipName), "Clip Name can't be empty")]
        [EnableIf(nameof(ClipAssignmentType), ClipAssignmentType.Name)]
        public string ClipName;
        private bool ValidateClipName => !string.IsNullOrEmpty(ClipName);

        [Title("Mixer")]

        public AudioMixerGroup MixerGroup;

        [Title("Type")]

        [EnumButtons]
        public SourceType SourceType;

        [EnableIf(nameof(SourceType), SourceType.Positional)]
        public Vector3 Position;

        [EnableIf(nameof(SourceType), SourceType.Attached)]
        public bool AssignTargetAtRuntime;

        // TODO: Replace with a single EnableIf when implemented
        [ValidateInput(nameof(ValidateTarget))]
        [EnableIf(nameof(SourceType), SourceType.Attached)]
        [EnableIf(nameof(AssignTargetAtRuntime), false)]
        public GameObject Target;
        private bool ValidateTarget =>
            SourceType != SourceType.Attached || AssignTargetAtRuntime || Target;

        [Title("Settings")]

        public bool UseSettingsPreset;

        [EnableIf(nameof(UseSettingsPreset), false)]
        public ItemSettings Settings;

        [ValidateInput(nameof(ValidateSettingsPreset))]
        [EnableIf(nameof(UseSettingsPreset), true)]
        [PreviewScriptable]
        public ItemSettingsPreset SettingsPreset;
        private bool ValidateSettingsPreset => !UseSettingsPreset || SettingsPreset;
    }
}