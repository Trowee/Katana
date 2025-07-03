using System;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioManager.Tweaks
{
    [Serializable]
    public class MixerGroupTweak : ITweak<AudioSource>
    {
        public AudioMixerGroup MixerGroup;
        
        public void Apply(AudioSource source) => source.outputAudioMixerGroup = MixerGroup;
    }
}