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
        public int Priority = 128;
        
        [Title("Volume")]
        [MinMax(0, 1, "")]
        public Vector2 Volume = Vector2.one;
        
        [Title("Pitch")]
        [MinMax(-3, 3, "")]
        public Vector2 Pitch = Vector2.one;
        
        [Title("Stereo Pan")]
        [MinMax(-1, 1, "")]
        public Vector2 StereoPan;
        
        [Title("Spatial Blend Range")]
        [MinMax(0, 1, "")]
        public Vector2 SpatialBlendRange;
        
        [Title("Reverb Zone Mix Range")]
        [MinMax(0, 1.1f, "")]
        public Vector2 ReverbZoneMixRange = Vector2.one;
        
        [FoldoutGroup("3D")]
        [Title("DopplerLevel Range")]
        [MinMax(0, 5, "")]
        public Vector2 DopplerLevelRange = Vector2.one;
        
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
        public Vector2 DistanceRange = new(1, 500);

        public AudioSource ApplyToSource(AudioSource source)
        {
            source.bypassEffects = BypassEffects;
            source.bypassListenerEffects = BypassListenerEffects;
            source.bypassReverbZones = BypassReverbZones;
            source.loop = Loop;
            source.priority = Priority;
            
            source.volume = Random.Range(Volume.x, Volume.y);
            source.pitch = Random.Range(Pitch.x, Pitch.y);
            source.panStereo = Random.Range(StereoPan.x, StereoPan.y);
            source.spatialBlend = Random.Range(SpatialBlendRange.x, SpatialBlendRange.y);
            source.reverbZoneMix = Random.Range(ReverbZoneMixRange.x, ReverbZoneMixRange.y);
            
            source.dopplerLevel = Random.Range(DopplerLevelRange.x, DopplerLevelRange.y);
            source.spread = Random.Range(SpreadRange.x, SpreadRange.y);
            source.rolloffMode = RolloffMode;
            source.minDistance = DistanceRange.x;
            source.maxDistance = DistanceRange.y;

            return source;
        }
    }
}