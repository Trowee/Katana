using System;
using UnityEngine;
using ArtificeToolkit.Attributes;
using Assets.NnUtils.Scripts.MinMax;

namespace Assets.Scripts.Audio
{
    [Serializable]
    public class ItemSettings
    {
        public bool BypassEffects;
        
        [EnableIf(nameof(BypassEffects), true)]
        public bool BypassListenerEffects;
        
        public bool BypassReverbZones;

        public bool Loop;

        [UnityEngine.Range(0, 256)]
        public int Priority = 128;
        
        [MinMax(0, 1)]
        public Vector2 Volume = Vector2.one;
        
        [MinMax(-3, 3)]
        public Vector2 Pitch = Vector2.one;
        
        [MinMax(-1, 1)]
        public Vector2 StereoPan;
        
        [MinMax(0, 1)]
        public Vector2 SpatialBlendRange;
        
        [MinMax(0, 1.1f)]
        public Vector2 ReverbZoneMixRange = Vector2.one;
        
        [FoldoutGroup("3D")]
        [MinMax(0, 5)]
        public Vector2 DopplerLevelRange = Vector2.one;
        
        [FoldoutGroup("3D")]
        [MinMax(0, 360)]
        public Vector2 SpreadRange;
        
        [FoldoutGroup("3D")]
        [EnumButtons]
        public AudioRolloffMode RolloffMode;
        
        [FoldoutGroup("3D")]
        [MinMax(0, 10000)]
        public Vector2 DistanceRange = new(1, 500);
    }
}