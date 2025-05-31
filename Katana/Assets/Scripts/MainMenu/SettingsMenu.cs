using System;
using System.Collections;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using Assets.Scripts.Player.Camera;
using NnUtils.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MainMenu
{
    public class SettingsMenu : MonoBehaviour
    {
        private static SettingsManager Settings => GameManager.SettingsManager;
        
        [FoldoutGroup("Graphics"), SerializeField]
        private TMP_Dropdown _perspective;
        [FoldoutGroup("Graphics"), SerializeField]
        private Slider _motionBlur;
        [FoldoutGroup("Graphics"), SerializeField]
        private Toggle _useDOF;
        
        [FoldoutGroup("Audio"), SerializeField]
        private Slider _masterVolume;
        [FoldoutGroup("Audio"), SerializeField]
        private Slider _sfxVolume;
        [FoldoutGroup("Audio"), SerializeField]
        private Slider _musicVolume;
        
        [FoldoutGroup("Tabs"), SerializeField]
        private RectTransform _tabsPanel;
        [FoldoutGroup("Tabs"), SerializeField]
        private float _animationTime;
        [FoldoutGroup("Tabs"), SerializeField]
        private Easings.Type _animationEasing;
        [FoldoutGroup("Tabs"), SerializeField]
        private AnimationCurve _animationCurve;
        
        private float _tabSize;
        
        private void Start()
        {
            _tabSize = _tabsPanel.rect.width;
            UpdateUI();
        }

        private void UpdateUI()
        {
            _useDOF.isOn      = Settings.UseDOF;
            _motionBlur.value = Settings.MotionBlur;
            _perspective.value =
                Array.IndexOf(Enum.GetValues(typeof(Perspective)), Settings.Perspective);
            _masterVolume.value = Settings.MasterVolume;
            _sfxVolume.value    = Settings.SFXVolume;
            _musicVolume.value  = Settings.MusicVolume;
        }

        public void SwitchTab(int tabIndex) =>
            this.RestartRoutine(ref _switchTabRoutine, SwitchTabRoutine(-tabIndex * _tabSize));

        private Coroutine _switchTabRoutine;
        private IEnumerator SwitchTabRoutine(float targetX)
        {
            var startPos = _tabsPanel.anchoredPosition;
            var targetPos = startPos;
            targetPos.x = targetX;
            
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = _animationCurve.Evaluate(
                    Misc.Tween(ref lerpPos, _animationTime, _animationEasing, true));
                _tabsPanel.anchoredPosition = Vector2.LerpUnclamped(startPos, targetPos, t);
                yield return null;
            }
            
            _switchTabRoutine = null;
        }

        public void SetDOF(bool value) => Settings.ChangeDOF(value);
        public void SetMotionBlur(float value) => Settings.ChangeMotionBlur(value);
        public void SetPerspective(int value) => Settings.ChangePerspective(value);
        public void SetMasterVolume(float value) => Settings.ChangeMasterVolume(value);
        public void SetSFXVolume(float value) => Settings.ChangeSFXVolume(value);
        public void SetMusicVolume(float value) => Settings.ChangeMusicVolume(value);
    }
}