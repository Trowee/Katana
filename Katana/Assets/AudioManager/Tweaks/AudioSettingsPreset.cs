using AudioManager.Tweaks;
using UnityEngine;

namespace AudioManager
{
    [CreateAssetMenu(fileName = "AudioSettingsPreset",
                     menuName = "NnUtils/Audio Manager/Tweaks Preset")]
    public class AudioSettingsPreset : ScriptableObject
    {
        public AudioTweaks Tweaks;
    }
}