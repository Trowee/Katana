using System;
using System.Linq;
using System.Threading.Tasks;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Player.Camera;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Core
{
    public class SettingsManager : MonoBehaviour
    {
        public bool IsRebinding;

        [SerializeField, Required, FoldoutGroup("Volume Profiles")]
        private VolumeProfile _dofProfile;
        [SerializeField, Required, FoldoutGroup("Volume Profiles")]
        private VolumeProfile _menuDOFProfile;
        [SerializeField, Required, FoldoutGroup("Volume Profiles")]
        private VolumeProfile _motionBlurProfile;

        [SerializeField, Required, FoldoutGroup("Audio")]
        private AudioMixerGroup _master;
        [SerializeField, Required, FoldoutGroup("Audio")]
        private AudioMixerGroup _sfx;
        [SerializeField, Required, FoldoutGroup("Audio")]
        private AudioMixerGroup _music;

        private void Awake() => LoadSettings();

        private async void LoadSettings()
        {
            UseDOF = PlayerPrefs.GetInt("UseDOF", 1) == 1;
            MotionBlur = PlayerPrefs.GetFloat("MotionBlur", 1);
            Perspective = (Perspective)Enum.Parse(typeof(Perspective),
                PlayerPrefs.GetString("Perspective", "First"));

            var localeCode = PlayerPrefs.GetString("Locale",
                                               LocalizationSettings.SelectedLocale.Identifier.Code);
            Locale = LocalizationSettings.AvailableLocales.Locales
                                          .FirstOrDefault(x => x.Identifier.Code == localeCode);

            // Takes care of updating the UI
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

            // Wait a bit before updating the audio mixer cuz unity devs are competent
            await Task.Delay(TimeSpan.FromSeconds(0.01f));

            MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        }

        #region DOF

        [Button, FoldoutGroup("Settings")]
        public void ChangeDOF(bool useDOF) => UseDOF = useDOF;

        private bool _useDOF;
        public bool UseDOF
        {
            get => _useDOF;
            private set
            {
                if (_useDOF == value) return;

                _useDOF = value;
                if (_menuDOFProfile && _dofProfile.TryGet(out DepthOfField dof))
                    dof.active = _useDOF;
                if (_menuDOFProfile && _menuDOFProfile.TryGet(out DepthOfField menuDOF))
                    menuDOF.active = _useDOF;

                PlayerPrefs.SetInt("UseDOF", UseDOF ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        #endregion

        #region MotionBlur

        [Button, FoldoutGroup("Settings")]
        public void ChangeMotionBlur(float mb) => MotionBlur = mb;

        private float _motionBlur;
        public float MotionBlur
        {
            get => _motionBlur;
            private set
            {
                if (Mathf.Approximately(_motionBlur, value)) return;

                _motionBlur = value;
                if (_motionBlurProfile && _motionBlurProfile.TryGet(out MotionBlur mb))
                    mb.intensity.value = _motionBlur;

                PlayerPrefs.SetFloat("MotionBlur", value);
                PlayerPrefs.Save();
            }
        }

        #endregion

        #region Perspective

        [Button, FoldoutGroup("Settings")]
        public void ChangePerspective(Perspective p) => Perspective = p;

        public void ChangePerspective(int p) => Perspective = (Perspective)p;

        private Perspective _perspective;
        public Perspective Perspective
        {
            get => _perspective;
            set
            {
                if (_perspective == value) return;
                _perspective = value;
                PlayerPrefs.SetString("Perspective", _perspective.ToString());
                PlayerPrefs.Save();
            }
        }

        #endregion

        #region Locales

        private Locale _locale;
        public Locale Locale
        {
            get => _locale;
            set
            {
                if (_locale == value) return;
                _locale = value;
                LocalizationSettings.SelectedLocale = _locale;
                PlayerPrefs.SetString("Locale", _locale.Identifier.Code);
                PlayerPrefs.Save();
            }
        }

        public void SetLocale(int index) =>
            Locale = LocalizationSettings.AvailableLocales.Locales[index];

        #endregion

        #region Volume

        private float GetVolume(float t) => Mathf.Lerp(-80, 0, t);
        [Button, FoldoutGroup("Settings/Audio")]
        public void ChangeMasterVolume(float vol) => MasterVolume = vol;
        [Button, FoldoutGroup("Settings/Audio")]
        public void ChangeSFXVolume(float vol) => SFXVolume = vol;
        [Button, FoldoutGroup("Settings/Audio")]
        public void ChangeMusicVolume(float vol) => MusicVolume = vol;

        private float _masterVolume;
        public float MasterVolume
        {
            get => _masterVolume;
            private set
            {
                if (Mathf.Approximately(_masterVolume, value)) return;
                _masterVolume = value;
                _master.audioMixer.SetFloat("MasterVolume", GetVolume(value));
                PlayerPrefs.SetFloat("MasterVolume", value);
                PlayerPrefs.Save();
            }
        }

        private float _sfxVolume;
        public float SFXVolume
        {
            get => _sfxVolume;
            private set
            {
                if (Mathf.Approximately(_sfxVolume, value)) return;
                _sfxVolume = value;
                _sfx.audioMixer.SetFloat("SFXVolume", GetVolume(value));
                PlayerPrefs.SetFloat("SFXVolume", value);
                PlayerPrefs.Save();
            }
        }

        private float _musicVolume;
        public float MusicVolume
        {
            get => _musicVolume;
            private set
            {
                if (Mathf.Approximately(_musicVolume, value)) return;
                _musicVolume = value;
                _music.audioMixer.SetFloat("MusicVolume", GetVolume(value));
                PlayerPrefs.SetFloat("MusicVolume", value);
                PlayerPrefs.Save();
            }
        }

        #endregion
    }
}