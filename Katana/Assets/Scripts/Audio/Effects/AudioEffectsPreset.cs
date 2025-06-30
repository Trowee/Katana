using UnityEngine;

namespace Assets.Scripts.Audio.Effects
{
    
    [CreateAssetMenu(fileName = "AudioEffectsPreset",
                     menuName = "NnUtils/Audio Manager/Effects Preset")]
    public class AudioEffectsPreset : ScriptableObject
    {
        public AudioEffects Effects;
    }
}