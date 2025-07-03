using System;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using AudioManager.Effects;
using AudioManager.Tweaks;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioManager
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

        [HorizontalGroup("Settings")]
        [Title("Scaled")]
        [HideLabel]
        [Optional(nameof(OverrideScaled), "")]
        public bool Scaled;

        [HideInInspector]
        public bool OverrideLoop;
        
        [HorizontalGroup("Settings")]
        [HideLabel]
        [Title("Loop")]
        [Optional(nameof(OverrideLoop), "")]
        public bool Loop;

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
        public bool AsChildObject;

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

        [TabGroup("Fade In")]
        [Optional(nameof(OverrideFadeIn), displayCheckbox: false)]
        public bool FadeInScale;

        [TabGroup("Fade In")]
        [Optional(nameof(OverrideFadeIn), displayCheckbox: false)]
        public bool FadeInScaleWithPitch;

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

        [TabGroup("Fade Out")]
        [Optional(nameof(OverrideFadeOut), displayCheckbox: false)]
        public bool FadeOutScale;

        [TabGroup("Fade Out")]
        [Optional(nameof(OverrideFadeOut), displayCheckbox: false)]
        public bool FadeOutScaleWithPitch;

        public bool ReloadTweaksEveryPlay;

        [SerializeReference]
        [ForceArtifice]
        public List<ITweak<AudioSource>> Tweaks;

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
            sourceType: SourceType.Manager,
            reuseSource: true,
            overridePlayOnAwake: true,
            playOnAwake: resourceItem.PlayOnAwake,
            overrideLoop: true,
            loop: resourceItem.Loop,
            overrideScaled: true,
            scaled: resourceItem.Scaled,
            position: resourceItem.Position,
            overrideFadeIn: true,
            fadeIn: resourceItem.FadeIn,
            fadeInTime: resourceItem.FadeInTime,
            fadeInEasing: resourceItem.FadeInEasing,
            fadeInScale: resourceItem.FadeInScale,
            fadeInScaleWithPitch:  resourceItem.FadeInScaleWithPitch,
            overrideFadeOut: true,
            fadeOut: resourceItem.FadeOut,
            fadeOutTime: resourceItem.FadeOutTime,
            fadeOutEasing: resourceItem.FadeOutEasing,
            fadeOutScale: resourceItem.FadeOutScale,
            fadeOutScaleWithPitch: resourceItem.FadeOutScaleWithPitch,
            tweaks: resourceItem.Tweaks,
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
                         SourceType sourceType = default,
                         bool reuseSource = true,
                         bool overridePlayOnAwake = false,
                         bool playOnAwake = false,
                         bool overrideLoop = false,
                         bool loop = false,
                         bool overrideScaled = false,
                         bool scaled = true,
                         bool destroySourceOnFinished = false,
                         bool destroyTargetOnFinished = false,
                         Vector3 position = default,
                         bool asChildObject = true,
                         bool assignTargetAtRuntime = false,
                         GameObject target = null,
                         bool overrideFadeIn = false,
                         bool fadeIn = false,
                         float fadeInTime = 0,
                         Easings.Type fadeInEasing = Easings.Type.Linear,
                         bool fadeInScale = true,
                         bool fadeInScaleWithPitch = true,
                         bool overrideFadeOut = false,
                         bool fadeOut = false,
                         float fadeOutTime = 0,
                         Easings.Type fadeOutEasing = Easings.Type.Linear,
                         bool fadeOutScale = true,
                         bool fadeOutScaleWithPitch = true,
                         bool reloadTweaksEveryPlay = true,
                         List<ITweak<AudioSource>> tweaks = null,
                         bool overrideEffects = false,
                         bool useEffectsPreset = true,
                         AudioEffects audioEffects = null,
                         AudioEffectsPreset audioEffectsPreset = null)
        {
            ReuseSource = reuseSource;
            ResourceAssignmentType = resourceAssignmentType;
            AudioResourceItem = audioResourceItem;
            AudioResource = audioResource;
            AudioResourceName = audioResourceName;
            SourceType = sourceType;
            OverridePlayOnAwake = overridePlayOnAwake;
            PlayOnAwake = playOnAwake;
            OverrideLoop = overrideLoop;
            Loop = loop;
            OverrideScaled = overrideScaled;
            Scaled = scaled;
            DestroySourceOnFinished = destroySourceOnFinished;
            DestroyTargetOnFinished = destroyTargetOnFinished;
            Position = position;
            AsChildObject = asChildObject;
            AssignTargetAtRuntime = assignTargetAtRuntime;
            Target = target;
            OverrideFadeIn = overrideFadeIn;
            FadeIn = fadeIn;
            FadeInTime = fadeInTime;
            FadeInEasing = fadeInEasing;
            FadeInScale = fadeInScale;
            FadeInScaleWithPitch = fadeInScaleWithPitch;
            OverrideFadeOut = overrideFadeOut;
            FadeOut = fadeOut;
            FadeOutTime = fadeOutTime;
            FadeOutEasing = fadeOutEasing;
            FadeOutScale = fadeOutScale;
            FadeOutScaleWithPitch = fadeOutScaleWithPitch;
            ReloadTweaksEveryPlay = reloadTweaksEveryPlay;
            Tweaks = tweaks;
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
                ResourceAssignmentType.Manual => throw new(
                                                     "(Audio Item) ResourceAssignmentType must not be set to 'Manual' at the time of calling the GetAudioResource function"),
                _ => throw new ArgumentOutOfRangeException()
            };

        public AudioSource ApplySettingsToSource(AudioSource source)
        {
            if (OverridePlayOnAwake) source.playOnAwake = PlayOnAwake;
            if (OverrideLoop) source.loop = Loop;
            source.resource = GetAudioResource(source);
            Tweaks.ForEach(t => t?.Apply(source));
            return source;
        }
    }
}