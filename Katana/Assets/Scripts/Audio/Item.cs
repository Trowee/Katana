using System;
using ArtificeToolkit.Attributes;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Audio
{
    // TODO: Replace ValidateInput with Required when fixed for EnableIf
    [Serializable]
    public class Item
    {
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

        public AudioMixerGroup MixerGroup;

        public Type Type;

        [EnableIf(nameof(Type), Type.Positional)]
        public Vector3 Position;

        [EnableIf(nameof(Type), Type.Attached)]
        public bool AssignTargetAtRuntime;
        
        // TODO: Replace with a single EnableIf when implemented
        [ValidateInput(nameof(ValidateTarget))]
        [EnableIf(nameof(Type), Type.Attached)]
        [EnableIf(nameof(AssignTargetAtRuntime), false)]
        public GameObject Target;
        private bool ValidateTarget => Type != Type.Attached || AssignTargetAtRuntime || Target;
        
        public bool UseSettingsPreset;
        
        [EnableIf(nameof(UseSettingsPreset), false)]
        public AudioSettings Settings;
        
        [ValidateInput(nameof(ValidateSettingsPreset))]
        [EnableIf(nameof(UseSettingsPreset), true)]
        [PreviewScriptable]
        public ItemSettingsPreset SettingsPreset;
        private bool ValidateSettingsPreset =>  !UseSettingsPreset || SettingsPreset;
    }
}