using System;
using ArtificeToolkit.Attributes;

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

        [Range(0, 256)]
        public int Priority = 128;
        
        [Range(0, 1)]
        public float Volume = 1;
        
        [Range(-3, 3)]
        public float Pitch = 1;
        
        [Range(-1, 1)]
        public float StereoPan;
        
        [Range(0, 1)]
        public float SpatialBlend;
        
        [Range(0, 1.1f)]
        public float ReverbZoneMix = 1;
        
        [FoldoutGroup("3D")]
        [Range(0, 5)]
        public float DopplerLevel = 1;
        
        [FoldoutGroup("3D")]
        [Range(0, 360)]
        public float Spread;
        
        [FoldoutGroup("3D")]
        public UnityEngine.AudioRolloffMode RolloffMode;
        
        [FoldoutGroup("3D")]
        public UnityEngine.Vector2 DistanceRange = new(1, 500);
    }
}