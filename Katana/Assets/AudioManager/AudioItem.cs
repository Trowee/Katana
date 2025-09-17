using System;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using AudioManager.Effects;
using UnityEngine;
using UnityEngine.Audio;

#if STEAMAUDIO_ENABLED

using SteamAudio;
using Vector3 = UnityEngine.Vector3;

#endif

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

        [EnableIf(nameof(UseEffectsPreset), true)]
        [Required]
        [PreviewScriptable]
        public AudioEffectsPreset AudioEffectsPreset;

#if STEAMAUDIO_ENABLED

        [Title("Steam Audio")]
        public bool OverrideSteamAudioSource = false;

        [EnableIf(nameof(OverrideSteamAudioSource), true)]
        public bool UseSteamAudioSource = false;

        [EnableIf(nameof(OverrideSteamAudioSource), true)]
        [Required]
        public SteamAudioSource SteamAudioSourcePreset;

#endif

        public AudioItem() : this(ResourceAssignmentType.ResourceItem)
        {
        }

        public AudioItem(AudioItem item) => new AudioItem().Copy(item);

        public AudioItem Copy(AudioItem item)
        {
            ResourceAssignmentType = item.ResourceAssignmentType;
            AudioResourceItem = item.AudioResourceItem;
            AudioResource = item.AudioResource;
            AudioResourceName = item.AudioResourceName;
            SourceType = item.SourceType;
            ReuseSource = item.ReuseSource;
            OverridePlayOnAwake = item.OverridePlayOnAwake;
            PlayOnAwake = item.PlayOnAwake;
            OverrideLoop = item.OverrideLoop;
            Loop = item.Loop;
            OverrideScaled = item.OverrideScaled;
            Scaled = item.Scaled;
            DestroySourceOnFinished = item.DestroySourceOnFinished;
            DestroyTargetOnFinished = item.DestroyTargetOnFinished;
            Position = item.Position;
            AsChildObject = item.AsChildObject;
            AssignTargetAtRuntime = item.AssignTargetAtRuntime;
            Target = item.Target;
            OverrideFadeIn = item.OverrideFadeIn;
            FadeIn = item.FadeIn;
            FadeInTime = item.FadeInTime;
            FadeInEasing = item.FadeInEasing;
            FadeInScale = item.FadeInScale;
            FadeInScaleWithPitch = item.FadeInScaleWithPitch;
            OverrideFadeOut = item.OverrideFadeOut;
            FadeOut = item.FadeOut;
            FadeOutTime = item.FadeOutTime;
            FadeOutEasing = item.FadeOutEasing;
            FadeOutScale = item.FadeOutScale;
            FadeOutScaleWithPitch = item.FadeOutScaleWithPitch;
            ReloadTweaksEveryPlay = item.ReloadTweaksEveryPlay;
            Tweaks = item.Tweaks;
            UseEffectsPreset = item.UseEffectsPreset;
            AudioEffects = item.AudioEffects;
            AudioEffectsPreset = item.AudioEffectsPreset;

#if STEAMAUDIO_ENABLED

            OverrideSteamAudioSource = item.OverrideSteamAudioSource;
            UseSteamAudioSource = item.UseSteamAudioSource;
            SteamAudioSourcePreset = item.SteamAudioSourcePreset;

#endif
            return this;
        }

        public AudioItem(AudioResourceItem resourceItem) => new AudioItem().Copy(resourceItem);

        public AudioItem Copy(AudioResourceItem resourceItem)
        {
            ResourceAssignmentType = ResourceAssignmentType.ResourceItem;
            AudioResourceItem = resourceItem;
            SourceType = resourceItem.SourceType;
            OverridePlayOnAwake = true;
            PlayOnAwake = resourceItem.PlayOnAwake;
            OverrideLoop = true;
            Loop = resourceItem.Loop;
            OverrideScaled = true;
            Scaled = resourceItem.Scaled;
            Position = resourceItem.Position;
            OverrideFadeIn = true;
            FadeIn = resourceItem.FadeIn;
            FadeInTime = resourceItem.FadeInTime;
            FadeInEasing = resourceItem.FadeInEasing;
            FadeInScale = resourceItem.FadeInScale;
            FadeInScaleWithPitch = resourceItem.FadeInScaleWithPitch;
            OverrideFadeOut = true;
            FadeOut = resourceItem.FadeOut;
            FadeOutTime = resourceItem.FadeOutTime;
            FadeOutEasing = resourceItem.FadeOutEasing;
            FadeOutScale = resourceItem.FadeOutScale;
            FadeOutScaleWithPitch = resourceItem.FadeOutScaleWithPitch;
            Tweaks = resourceItem.Tweaks;
            UseEffectsPreset = resourceItem.UseEffectsPreset;
            AudioEffects = resourceItem.AudioEffects;
            AudioEffectsPreset = resourceItem.AudioEffectsPreset;

#if STEAMAUDIO_ENABLED

            OverrideSteamAudioSource = true;
            UseSteamAudioSource = resourceItem.UseSteamAudioSource;
            SteamAudioSourcePreset = resourceItem.SteamAudioSourcePreset;

#endif
            return this;
        }


        public AudioItem(
            ResourceAssignmentType resourceAssignmentType = default,
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
            AudioEffectsPreset audioEffectsPreset = null
#if STEAMAUDIO_ENABLED

            ,
            bool overrideSteamAudioSource = false,
            bool useSteamAudioSource = false,
            SteamAudioSource steamAudioSourcePreset = null

#endif
        )
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

#if STEAMAUDIO_ENABLED

            OverrideSteamAudioSource = overrideSteamAudioSource;
            UseSteamAudioSource = useSteamAudioSource;
            SteamAudioSourcePreset = steamAudioSourcePreset;

#endif
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

#if STEAMAUDIO_ENABLED

        public SteamAudioSource ApplySettingsToSteamAudioSource(SteamAudioSource target)
        {
            var flags =
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly;

            foreach (var pinfo in typeof(SteamAudioSource).GetProperties(flags))
                if (pinfo.CanWrite)
                    try
                    {
                        pinfo.SetValue(target, pinfo.GetValue(SteamAudioSourcePreset, null), null);
                    }
                    catch { }

            foreach (var finfo in typeof(SteamAudioSource).GetFields(flags))
                if (!finfo.IsStatic) finfo.SetValue(target, finfo.GetValue(SteamAudioSourcePreset));

            return target;
        }

#endif
    }
}