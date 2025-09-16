using System;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using AudioManager.Effects;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioManager
{
    [Serializable]
    public class AudioItem
    {
        [Title("Resource Assignment Type")]
        [HideLabel]
        [EnumToggle]
        public ResourceAssignmentType ResourceAssignmentType;

        [HideLabel]
        [EnableIf(nameof(ResourceAssignmentType), ResourceAssignmentType.ResourceItem)]
        [Required]
        [PreviewScriptable]
        public AudioResourceItem AudioResourceItem;

        [HideLabel]
        [EnableIf(nameof(ResourceAssignmentType), ResourceAssignmentType.Resource)]
        [Required]
        public AudioResource AudioResource;

        [HideLabel]
        [EnableIf(nameof(ResourceAssignmentType), ResourceAssignmentType.Name)]
        [ValidateInput(nameof(ValidateResourceName), "Audio Resource Name can't be empty")]
        public string AudioResourceName;
        private bool ValidateResourceName => !string.IsNullOrEmpty(AudioResourceName);

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

        [Title("Source")]
        [HideLabel]
        [EnumToggle]
        public SourceType SourceType;

        [EnableIf(nameof(SourceType), SourceType.Positional)]
        public Vector3 Position;

        [EnableIf(nameof(SourceType), SourceType.Object)]
        public bool AsChildObject;

        [EnableIf(nameof(SourceType), SourceType.Object)]
        public bool AssignTargetAtRuntime;

        [HideLabel]
        [EnableIf(nameof(AssignTargetAtRuntime), false)]
        [EnableIf(nameof(SourceType), SourceType.Object)]
        [Required]
        public GameObject Target;

        [HorizontalGroup("Fade")]
        [Title("Fade In")]
        [HideLabel]
        public bool OverrideFadeIn;

        [HorizontalGroup("Fade")]
        [Title("Fade Out")]
        [HideLabel]
        public bool OverrideFadeOut;

        [EnableIf(nameof(OverrideFadeIn), true)]
        [TabGroup("Fade In")]
        public bool FadeIn;

        [EnableIf(nameof(OverrideFadeIn), true)]
        [TabGroup("Fade In")]
        public float FadeInTime;

        [EnableIf(nameof(OverrideFadeIn), true)]
        [TabGroup("Fade In")]
        public Easings.Type FadeInEasing;

        [EnableIf(nameof(OverrideFadeIn), true)]
        [TabGroup("Fade In")]
        public bool FadeInScale;

        [EnableIf(nameof(OverrideFadeIn), true)]
        [TabGroup("Fade In")]
        public bool FadeInScaleWithPitch;

        [EnableIf(nameof(OverrideFadeOut), true)]
        [TabGroup("Fade Out")]
        public bool FadeOut;

        [EnableIf(nameof(OverrideFadeOut), true)]
        [TabGroup("Fade Out")]
        public float FadeOutTime;

        [EnableIf(nameof(OverrideFadeOut), true)]
        [TabGroup("Fade Out")]
        public Easings.Type FadeOutEasing;

        [EnableIf(nameof(OverrideFadeOut), true)]
        [TabGroup("Fade Out")]
        public bool FadeOutScale;

        [EnableIf(nameof(OverrideFadeOut), true)]
        [TabGroup("Fade Out")]
        public bool FadeOutScaleWithPitch;

        [Title("Tweaks")]
        public bool ReloadTweaksEveryPlay;

        [SerializeReference]
        [ForceArtifice]
        public List<IAppliable<AudioSource>> Tweaks;

        [Title("Effects")]
        public bool UseEffectsPreset;

        [EnableIf(nameof(UseEffectsPreset), false)]
        public AudioEffects AudioEffects;

        public AudioEffects Effects => UseEffectsPreset ? AudioEffectsPreset.Effects : AudioEffects;

        [Required]
        [EnableIf(nameof(UseEffectsPreset), true)]
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
            fadeInScaleWithPitch: resourceItem.FadeInScaleWithPitch,
            overrideFadeOut: true,
            fadeOut: resourceItem.FadeOut,
            fadeOutTime: resourceItem.FadeOutTime,
            fadeOutEasing: resourceItem.FadeOutEasing,
            fadeOutScale: resourceItem.FadeOutScale,
            fadeOutScaleWithPitch: resourceItem.FadeOutScaleWithPitch,
            tweaks: resourceItem.Tweaks,
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
                         List<IAppliable<AudioSource>> tweaks = null,
                         bool useEffectsPreset = false,
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