using ArtificeToolkit.Attributes;
using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Audio
{
    [CreateAssetMenu(fileName = "ResourceItem",
                     menuName = "NnUtils/Audio Manager/Audio Resource Item")]
    public class AudioResourceItem : ScriptableObject
    {
        public string Name;
        public AudioResource Resource;

        [Title("Play On Awake")]
        [HideLabel]
        public bool PlayOnAwake;
        
        [Title("Mixer Group")]
        [HideLabel]
        public AudioMixerGroup MixerGroup;

        [Title("Source")]
        [ValidateInput(nameof(ValidateSourceType),
                       "SourceType can't be set to Object on an AudioResourceItem")]
        [HideLabel]
        [EnumToggle]
        public SourceType SourceType;

        private bool ValidateSourceType => SourceType != SourceType.Object;

        [HideLabel]
        [EnableIf(nameof(SourceType), SourceType.Positional)]
        public Vector3 Position;

        [Title("Settings")]
        public bool ReloadSettingsEveryPlay;
        public bool UseItemSettingsPreset;

        [EnableIf(nameof(UseItemSettingsPreset), false)]
        public AudioItemSettings ItemSettings;

        [ValidateInput(nameof(ValidateItemSettingsPreset))]
        [EnableIf(nameof(UseItemSettingsPreset), true)]
        [PreviewScriptable]
        public AudioItemSettingsPreset ItemSettingsPreset;
        private bool ValidateItemSettingsPreset => !UseItemSettingsPreset || ItemSettingsPreset;

        public AudioSource ApplySettingsToSource(AudioSource source)
        {
            source.playOnAwake = PlayOnAwake;
            source.outputAudioMixerGroup = MixerGroup;
            source.resource = Resource;
            return (UseItemSettingsPreset ? ItemSettingsPreset.Settings : ItemSettings)
                .ApplyToSource(source);
        }
    }
}