using UnityEngine;

namespace AudioManager
{
    [CreateAssetMenu(fileName = "AudioSettingsPreset",
                     menuName = "NnUtils/Audio Manager/Settings Preset")]
    public class AudioSettingsPreset : ScriptableObject
    {
        public AudioSettings Settings;
    }
}