using System;
using UnityEngine;
using ArtificeToolkit.Attributes;
using Assets.NnUtils.Scripts.MinMax;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Audio
{
    [Serializable]
    public class AudioItemSettings
    {
        [HideInInspector]
        public bool OverrideBypassEffects;
        
        [HorizontalGroup("1")]
        [Title("Bypass Effects")]
        [Optional(nameof(OverrideBypassEffects), "")]
        public bool BypassEffects;
        
        [HideInInspector]
        public bool OverrideBypassListenerEffects;
        
        [HorizontalGroup("1")]
        [EnableIf(nameof(BypassEffects), true)]
        [Title("Bypass Listener Effects")]
        [Optional(nameof(OverrideBypassListenerEffects), "")]
        public bool BypassListenerEffects;
        
        [HideInInspector]
        public bool OverrideBypassReverbZones;
        
        [HorizontalGroup("2")]
        [HideLabel]
        [Title("Bypass Reverb Zones")]
        [Optional(nameof(OverrideBypassReverbZones), "")]
        public bool BypassReverbZones;

        [HideInInspector]
        public bool OverrideLoop;
        
        [HorizontalGroup("2")]
        [HideLabel]
        [Title("Loop")]
        [Optional(nameof(OverrideLoop), "")]
        public bool Loop;

        [HideInInspector]
        public bool OverridePriority;
        
        [Title("Priority")]
        [Optional(nameof(OverridePriority), "")]
        [UnityEngine.Range(0, 256)]
        public int Priority;
        
        [HideInInspector]
        public bool OverrideVolume;
        
        [Title("Volume Range")]
        [Optional(nameof(OverrideVolume), "")]
        [MinMax(0, 1, "")]
        public Vector2 VolumeRange;
        
        [HideInInspector]
        public bool OverridePitch;
        
        [Title("Pitch Range")]
        [Optional(nameof(OverridePitch), "")]
        [MinMax(-3, 3, "")]
        public Vector2 PitchRange;
        
        [HideInInspector]
        public bool OverrideStereoPan;
        
        [Title("Stereo Pan Range")]
        [Optional(nameof(OverrideStereoPan), "")]
        [MinMax(-1, 1, "")]
        public Vector2 StereoPanRange;
        
        [HideInInspector]
        public bool OverrideSpatialBlend;
        
        [Title("Spatial Blend Range")]
        [Optional(nameof(OverrideSpatialBlend), "")]
        [MinMax(0, 1, "")]
        public Vector2 SpatialBlendRange;
        
        [HideInInspector]
        public bool OverrideReverbZoneMix;
        
        [Title("Reverb Zone Mix Range")]
        [Optional(nameof(OverrideReverbZoneMix), "")]
        [MinMax(0, 1.1f, "")]
        public Vector2 ReverbZoneMixRange;
        
        [HideInInspector]
        public bool OverrideDopplerLevel;
        
        [FoldoutGroup("3D")]
        [Title("Doppler Level Range")]
        [Optional(nameof(OverrideDopplerLevel), "")]
        [MinMax(0, 5, "")]
        public Vector2 DopplerLevelRange;

        [HideInInspector]
        public bool OverrideSpread;
        
        [FoldoutGroup("3D")]
        [Title("Spread Range")]
        [Optional(nameof(OverrideSpread), "")]
        [MinMax(0, 360, "")]
        public Vector2 SpreadRange;

        [FoldoutGroup("3D")]
        [HorizontalGroup("RolloffMode", 0.1f)]
        [Title("Rolloff")]
        [HideLabel]
        public bool OverrideRolloffMode;
        
        [FoldoutGroup("3D")]
        [HorizontalGroup("RolloffMode", 0.9f)]
        [Title("Mode")]
        [EnableIf(nameof(OverrideRolloffMode), true)]
        [EnumToggle]
        [HideLabel]
        public AudioRolloffMode RolloffMode;

        [HideInInspector]
        public bool OverrideDistanceRange;
        
        [FoldoutGroup("3D")]
        [Title("Distance Range")]
        [Optional(nameof(OverrideDistanceRange), "")]
        [MinMax(0, 10000, "")]
        public Vector2 DistanceRange;

        public AudioSource ApplyToSource(AudioSource source)
        {
            if (OverrideBypassEffects)
                source.bypassEffects = BypassEffects;
            if (OverrideBypassListenerEffects)
                source.bypassListenerEffects = BypassListenerEffects;
            if (OverrideBypassReverbZones)
                source.bypassReverbZones = BypassReverbZones;
            if (OverrideLoop)
                source.loop = Loop;
            if (OverridePriority)
                source.priority = Priority;
            
            if (OverrideVolume)
                source.volume = Random.Range(VolumeRange.x, VolumeRange.y);
            if (OverridePitch)
                source.pitch = Random.Range(PitchRange.x, PitchRange.y);
            if (OverrideStereoPan)
                source.panStereo = Random.Range(StereoPanRange.x, StereoPanRange.y);
            if (OverrideSpatialBlend)
                source.spatialBlend = Random.Range(SpatialBlendRange.x, SpatialBlendRange.y);
            if (OverrideReverbZoneMix)
                source.reverbZoneMix = Random.Range(ReverbZoneMixRange.x, ReverbZoneMixRange.y);
            
            if (OverrideDopplerLevel)
                source.dopplerLevel = Random.Range(DopplerLevelRange.x, DopplerLevelRange.y);
            if (OverrideSpread)
                source.spread = Random.Range(SpreadRange.x, SpreadRange.y);
            if (OverrideRolloffMode)
                source.rolloffMode = RolloffMode;
            if (OverrideDistanceRange)
            {
                source.minDistance = DistanceRange.x;
                source.maxDistance = DistanceRange.y;
            }

            return source;
        }

        public AudioItemSettings() : this(priority: 128,
                                          volumeRange: Vector2.one,
                                          pitchRange: Vector2.one,
                                          stereoPanRange: Vector2.zero,
                                          spatialBlendRange: Vector2.zero,
                                          reverbZoneMixRange: Vector2.one,
                                          dopplerLevelRange: Vector2.one,
                                          spreadRange: Vector2.zero,
                                          rolloffMode: AudioRolloffMode.Logarithmic,
                                          distanceRange: new(1, 500)) { }

        public AudioItemSettings(bool overrideBypassEffects = false,
                                 bool bypassEffects = false,
                                 bool overrideBypassListenerEffects = false,
                                 bool bypassListenerEffects = false,
                                 bool  overrideBypassReverbZones = false,
                                 bool bypassReverbZones = false,
                                 bool overrideLoop = false,
                                 bool loop = false,
                                 bool overridePriority = false,
                                 int priority = 128,
                                 bool overrideVolume = false,
                                 Vector2? volumeRange = null,
                                 bool overridePitch = false,
                                 Vector2? pitchRange = null,
                                 bool overrideStereoPan = false,
                                 Vector2? stereoPanRange = null,
                                 bool overrideSpatialBlend = false,
                                 Vector2? spatialBlendRange = null,
                                 bool overrideReverbZoneMix = false,
                                 Vector2? reverbZoneMixRange = null,
                                 bool overrideDopplerLevel = false,
                                 Vector2? dopplerLevelRange = null,
                                 bool overrideSpread = false,
                                 Vector2? spreadRange = null,
                                 bool overrideRolloffMode = false,
                                 AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic,
                                 bool overrideDistanceRange = false,
                                 Vector2? distanceRange = null)
        {
            OverrideBypassEffects = overrideBypassEffects;
            BypassEffects = bypassEffects;
            OverrideBypassListenerEffects = overrideBypassListenerEffects;
            BypassListenerEffects = bypassListenerEffects;
            OverrideBypassReverbZones = overrideBypassReverbZones;
            BypassReverbZones = bypassReverbZones;
            OverrideLoop = overrideLoop;
            Loop = loop;
            OverridePriority = overridePriority;
            Priority = priority;

            OverrideVolume = overrideVolume;
            VolumeRange = volumeRange ?? Vector2.one;
            OverridePitch = overridePitch;
            PitchRange = pitchRange ?? Vector2.one;
            OverrideStereoPan = overrideStereoPan;
            StereoPanRange = stereoPanRange ?? Vector2.zero;
            OverrideSpatialBlend = overrideSpatialBlend;
            SpatialBlendRange = spatialBlendRange ?? Vector2.zero;
            OverrideReverbZoneMix = overrideReverbZoneMix;
            ReverbZoneMixRange = reverbZoneMixRange ?? Vector2.zero;

            OverrideDopplerLevel = overrideDopplerLevel;
            DopplerLevelRange = dopplerLevelRange ?? Vector2.zero;
            OverrideSpread = overrideSpread;
            SpreadRange = spreadRange ?? Vector2.zero;
            OverrideRolloffMode = overrideRolloffMode;
            RolloffMode = rolloffMode;
            OverrideDistanceRange = overrideDistanceRange;
            DistanceRange = distanceRange ?? new(1, 500);
        }
    }
}