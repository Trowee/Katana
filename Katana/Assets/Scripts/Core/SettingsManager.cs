using System;
using System.Threading.Tasks;
using Alchemy.Inspector;
using Assets.Scripts.Player.Camera;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Core
{
    public class SettingsManager : MonoBehaviour
    {
        [FoldoutGroup("Volume Profiles"), SerializeField, Required]
        private VolumeProfile _dofProfile;
        [FoldoutGroup("Volume Profiles"), SerializeField, Required]
        private VolumeProfile _menuDOFProfile;
        [FoldoutGroup("Volume Profiles"), SerializeField, Required]
        private VolumeProfile _motionBlurProfile;
        
        [FoldoutGroup("Audio"), SerializeField, Required] private AudioMixerGroup _master;
        [FoldoutGroup("Audio"), SerializeField, Required] private AudioMixerGroup _sfx;
        [FoldoutGroup("Audio"), SerializeField, Required] private AudioMixerGroup _music;

        private void Awake() => LoadSettings();

        private async void LoadSettings()
        {
            UseDOF = PlayerPrefs.GetInt("UseDOF", 1) == 1;
            MotionBlur = PlayerPrefs.GetFloat("MotionBlur", 1);
            Perspective = (Perspective)Enum.Parse(typeof(Perspective),
                PlayerPrefs.GetString("Perspective", "First"));

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
        
        [FoldoutGroup("Settings"), Button]
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
            }
        }
        
        #endregion

        #region MotionBlur
        
        [FoldoutGroup("Settings"), Button]
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
            }
        }
        
        #endregion

        #region Perspective

        [FoldoutGroup("Settings"), Button]
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
            }
        }
        
        #endregion
        
        #region Volume
        
        private float GetVolume(float t) => Mathf.Lerp(-80, 0, t);
        [FoldoutGroup("Settings/Audio"), Button]
        public void ChangeMasterVolume(float vol) => MasterVolume = vol;
        [FoldoutGroup("Settings/Audio"), Button]
        public void ChangeSFXVolume(float vol) => SFXVolume = vol;
        [FoldoutGroup("Settings/Audio"), Button]
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
            }
        }

        #endregion
    }
}