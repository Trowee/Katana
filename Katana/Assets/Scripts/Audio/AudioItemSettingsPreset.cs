using UnityEngine;

namespace Assets.Scripts.Audio
{
    [CreateAssetMenu(fileName = "AudioItemSettingsPreset",
                     menuName = "NnUtils/Audio Manager/Item Settings Preset")]
    public class AudioItemSettingsPreset : ScriptableObject
    {
        public AudioItemSettings Settings;
    }
}