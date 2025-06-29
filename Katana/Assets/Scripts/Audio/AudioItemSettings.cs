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
        [HorizontalGroup("1")]
        [HideLabel]
        [Title("Bypass Effects")]
        public bool BypassEffects;
        
        [HorizontalGroup("1")]
        [EnableIf(nameof(BypassEffects), true)]
        [HideLabel]
        [Title("Bypass Listener Effects")]
        public bool BypassListenerEffects;
        
        [HorizontalGroup("2")]
        [HideLabel]
        [Title("Bypass Reverb Zones")]
        public bool BypassReverbZones;

        [HorizontalGroup("2")]
        [HideLabel]
        [Title("Loop")]
        public bool Loop;

        [Title("Priority")]
        [HideLabel]
        [UnityEngine.Range(0, 256)]
        public int Priority;
        
        [Title("Volume Range")]
        [MinMax(0, 1, "")]
        public Vector2 VolumeRange;
        
        [Title("Pitch Range")]
        [MinMax(-3, 3, "")]
        public Vector2 PitchRange;
        
        [Title("Stereo Pan Range")]
        [MinMax(-1, 1, "")]
        public Vector2 StereoPanRange;
        
        [Title("Spatial Blend Range")]
        [MinMax(0, 1, "")]
        public Vector2 SpatialBlendRange;
        
        [Title("Reverb Zone Mix Range")]
        [MinMax(0, 1.1f, "")]
        public Vector2 ReverbZoneMixRange;
        
        [FoldoutGroup("3D")]
        [Title("Doppler Level Range")]
        [MinMax(0, 5, "")]
        public Vector2 DopplerLevelRange;
        
        [FoldoutGroup("3D")]
        [Title("Spread Range")]
        [MinMax(0, 360, "")]
        public Vector2 SpreadRange;
        
        [FoldoutGroup("3D")]
        [Title("Rolloff Mode")]
        [HideLabel]
        [EnumToggle]
        public AudioRolloffMode RolloffMode;
        
        [FoldoutGroup("3D")]
        [Title("Distance Range")]
        [MinMax(0, 10000, "")]
        public Vector2 DistanceRange;

        public AudioSource ApplyToSource(AudioSource source)
        {
            source.bypassEffects = BypassEffects;
            source.bypassListenerEffects = BypassListenerEffects;
            source.bypassReverbZones = BypassReverbZones;
            source.loop = Loop;
            source.priority = Priority;
            
            source.volume = Random.Range(VolumeRange.x, VolumeRange.y);
            source.pitch = Random.Range(PitchRange.x, PitchRange.y);
            source.panStereo = Random.Range(StereoPanRange.x, StereoPanRange.y);
            source.spatialBlend = Random.Range(SpatialBlendRange.x, SpatialBlendRange.y);
            source.reverbZoneMix = Random.Range(ReverbZoneMixRange.x, ReverbZoneMixRange.y);
            
            source.dopplerLevel = Random.Range(DopplerLevelRange.x, DopplerLevelRange.y);
            source.spread = Random.Range(SpreadRange.x, SpreadRange.y);
            source.rolloffMode = RolloffMode;
            source.minDistance = DistanceRange.x;
            source.maxDistance = DistanceRange.y;

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

        public AudioItemSettings(bool bypassEffects = false,
                                 bool bypassListenerEffects = false,
                                 bool bypassReverbZones = false,
                                 bool loop = false,
                                 int priority = 128,
                                 Vector2? volumeRange = null,
                                 Vector2? pitchRange = null,
                                 Vector2? stereoPanRange = null,
                                 Vector2? spatialBlendRange = null,
                                 Vector2? reverbZoneMixRange = null,
                                 Vector2? dopplerLevelRange = null,
                                 Vector2? spreadRange = null,
                                 AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic,
                                 Vector2? distanceRange = null)
        {
            BypassEffects = bypassEffects;
            BypassListenerEffects = bypassListenerEffects;
            BypassReverbZones = bypassReverbZones;
            Loop = loop;
            Priority = priority;

            VolumeRange = volumeRange ?? Vector2.one;
            PitchRange = pitchRange ?? Vector2.one;
            StereoPanRange = stereoPanRange ?? Vector2.zero;
            SpatialBlendRange = spatialBlendRange ?? Vector2.zero;
            ReverbZoneMixRange = reverbZoneMixRange ?? Vector2.zero;

            DopplerLevelRange = dopplerLevelRange ?? Vector2.zero;
            SpreadRange = spreadRange ?? Vector2.zero;
            RolloffMode = rolloffMode;
            DistanceRange = distanceRange ?? new(1, 500);
        }
    }
}