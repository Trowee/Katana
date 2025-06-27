using UnityEngine;
using ArtificeToolkit;

namespace Assets.Scripts.Audio
{
    [CreateAssetMenu(fileName = "ItemSettingsPreset",
                     menuName = "NnUtils/AudioManager/ItemSettingsPreset")]
    public class ItemSettingsPreset : ScriptableObject
    {
        public ItemSettings Settings;
    }
}