using System;
using Assets.Scripts.PlayerCamera;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Core
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField] private VolumeProfile _dofProfile;
        [SerializeField] private VolumeProfile _motionBlurProfile;
        
        private bool _useDOF;
        public bool UseDOF
        {
            get => _useDOF;
            private set
            {
                if (_useDOF == value) return;
                _useDOF = value;
                PlayerPrefs.SetInt("UseDOF", UseDOF ? 1 : 0);
                if (_dofProfile.TryGet(out DepthOfField dof)) dof.active = _useDOF;
            }
        }

        private float _motionBlur;
        public float MotionBlur
        {
            get => _motionBlur;
            private set
            {
                if (Mathf.Approximately(_motionBlur, value)) return;
                _motionBlur = value;
                PlayerPrefs.SetFloat("MotionBlur", value);
                if (_motionBlurProfile.TryGet(out MotionBlur mb)) mb.intensity.value = _motionBlur;
            }
        }

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

        private float _masterVolume;
        public float MasterVolume
        {
            get => _masterVolume;
            private set
            {
                if (Mathf.Approximately(_masterVolume, value)) return;
                _masterVolume = value;
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
                PlayerPrefs.SetFloat("MusicVolume", value);
            }
        }

        private void Awake() => LoadSettings();

        private void LoadSettings()
        {
            UseDOF = PlayerPrefs.GetInt("UseDOF", 1) == 1;
            MotionBlur = PlayerPrefs.GetFloat("MotionBlur", 1);
            Perspective = (Perspective)Enum.Parse(typeof(Perspective), PlayerPrefs.GetString("Perspective", "First"));
            MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
            SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        }
        
        public void ChangeDOF(bool useDOF) => UseDOF = useDOF;
        public void ChangeMotionBlur(float mb) => MotionBlur = mb;
        public void ChangePerspective(int p) => Perspective = (Perspective)p;
        public void ChangeMasterVolume(float vol) => MasterVolume = vol;
        public void ChangeSFXVolume(float vol) => SFXVolume = vol;
        public void ChangeMusicVolume(float vol) => MusicVolume = vol;
    }
}