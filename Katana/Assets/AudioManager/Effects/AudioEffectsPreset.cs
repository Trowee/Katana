using UnityEngine;

namespace AudioManager.Effects
{
    [CreateAssetMenu(fileName = "AudioEffectsPreset",
                     menuName = "NnUtils/Audio Manager/Effects Preset")]
    public class AudioEffectsPreset : ScriptableObject
    {
        public AudioEffects Effects;
    }
}