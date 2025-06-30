using System;
using ArtificeToolkit.Attributes;
using NnUtils.Scripts;
using UnityEngine;

namespace Assets.Scripts.Audio.Effects
{
    [Serializable]
    public class Reverb : AudioEffect
    {
        [EnumButtons]
        public AudioReverbPreset Preset;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(-10000, 0)]
        public float DryLevel = 0;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(-10000, 0)]
        public float Room = 0;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(-10000, 0)]
        public float RoomHF = 0;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(-10000, 0)]
        public float RoomLF = 0;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(0.1f, 20)]
        public float DecayTime = 1;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(0.1f, 2)]
        public float DecayHFRatio = 0.5f;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(-10000, 1000)]
        public float ReflectionsLevel = -10000;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(0, 0.3f)]
        public float ReflectionsDelay = 0;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(-10000, 2000)]
        public float ReverbLevel = 0;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(0, 0.1f)]
        public float ReverbDelay = 0.04f;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(1000, 20000)]
        public float HFReference = 5000;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(20, 1000)]
        public float LFReference = 250;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(0, 100)]
        public float Diffusion = 100;
        
        [EnableIf(nameof(Preset), AudioReverbPreset.User)]
        [UnityEngine.Range(0, 100)]
        public float Density = 100;

        public override void ApplyEffect(AudioManagerItem item)
        {
            if (!Enabled)
            {
                ClearEffect(item);
                return;
            }

            var filter = item.gameObject.GetOrAddComponent<AudioReverbFilter>();
            item.EffectCounts[typeof(AudioReverbFilter)].Add(this);

            filter.reverbPreset = Preset;
            if (Preset != AudioReverbPreset.User) return;
            
            filter.dryLevel = DryLevel;
            filter.room = Room;
            filter.roomHF = RoomHF;
            filter.roomLF = RoomLF;
            filter.decayTime = DecayTime;
            filter.decayHFRatio = DecayHFRatio;
            filter.reflectionsLevel = ReflectionsLevel;
            filter.reflectionsDelay = ReflectionsDelay;
            filter.reverbLevel = ReverbLevel;
            filter.reverbDelay = ReverbDelay;
            filter.hfReference = HFReference;
            filter.lfReference = LFReference;
            filter.diffusion = Diffusion;
            filter.density = Density;
        }

        public override void ClearEffect(AudioManagerItem item) =>
            item.EffectCounts[typeof(AudioReverbFilter)].Remove(this);
    }
}