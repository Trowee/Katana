using ArtificeToolkit.Attributes;
using Assets.Scripts.Audio.Effects;
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

        [HideLabel]
        [EnableIf(nameof(SourceType), SourceType.Positional)]
        public Vector3 Position;

        [Title("Settings")]
        public bool ReloadSettingsEveryPlay;
        public bool UseSettingsPreset;

        [EnableIf(nameof(UseSettingsPreset), false)]
        public AudioSettings Settings;

        [ValidateInput(nameof(ValidateItemSettingsPreset))]
        [EnableIf(nameof(UseSettingsPreset), true)]
        [PreviewScriptable]
        public AudioSettingsPreset SettingsPreset;

        public bool UseEffectsPreset;

        [EnableIf(nameof(UseEffectsPreset), false)]
        public AudioEffects AudioEffects;

        [ValidateInput(nameof(ValidateEffectsPreset))]
        [EnableIf(nameof(UseEffectsPreset), true)]
        [PreviewScriptable]
        public AudioEffectsPreset AudioEffectsPreset;

        private bool ValidateSourceType => SourceType != SourceType.Object;
        private bool ValidateItemSettingsPreset => !UseSettingsPreset || SettingsPreset;
        private bool ValidateEffectsPreset => !UseEffectsPreset || AudioEffectsPreset;

        public AudioSource ApplySettingsToSource(AudioSource source)
        {
            source.playOnAwake = PlayOnAwake;
            source.outputAudioMixerGroup = MixerGroup;
            source.resource = Resource;
            return (UseSettingsPreset ? SettingsPreset.Settings : Settings)
                .ApplyToSource(source);
        }
    }
}