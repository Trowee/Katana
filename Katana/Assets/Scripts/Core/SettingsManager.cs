using System;
using Assets.Scripts.PlayerCamera;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class SettingsManager : MonoBehaviour
    {
        public bool UseDOF { get; private set; }
        public float MotionBlur { get; private set; }
        
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
        
        public float MasterVolume { get; private set; }
        public float SFXVolume { get; private set; }
        public float MusicVolume { get; private set; }

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
        
        public void SaveSettings()
        {
            PlayerPrefs.SetInt("UseDOF", UseDOF ? 1 : 0);
            PlayerPrefs.SetFloat("MotionBlur", MotionBlur);
            
            PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
            PlayerPrefs.SetFloat("SFXVolume", SFXVolume);
            PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        }
        
        public void ChangeDOF(bool useDOF) => UseDOF = useDOF;
        public void ChangeMotionBlur(float mb) => MotionBlur = mb;
        public void ChangeMasterVolume(float vol) => MasterVolume = vol;
        public void ChangeSFXVolume(float vol) => SFXVolume = vol;
        public void ChangeMusicVolume(float vol) => MusicVolume = vol;
    }
}