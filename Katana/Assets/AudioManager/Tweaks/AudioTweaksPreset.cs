using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using UnityEngine;

namespace AudioManager.Tweaks
{
    [CreateAssetMenu(fileName = "AudioTweaksPreset",
                     menuName = "NnUtils/Audio Manager/Tweaks Preset")]
    public class AudioTweaksPreset : ScriptableObject
    {
        [SerializeReference]
        [ForceArtifice]
        public List<IAppliable<AudioSource>> Tweaks;
        
        public void Apply(AudioSource target) => Tweaks.ForEach(t => t?.Apply(target));
    }
}