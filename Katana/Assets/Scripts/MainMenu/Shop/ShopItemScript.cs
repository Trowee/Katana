using System;
using System.Collections;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using Assets.Scripts.Items;
using AudioManager;
using EasyTextEffects;
using NnUtils.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using Color = UnityEngine.Color;

namespace Assets.Scripts.MainMenu.Shop
{
    [ExecuteAlways]
    public class ShopItemScript : MonoBehaviour
    {
        private static ItemManager ItemManager => GameManager.ItemManager;

        [FoldoutGroup("Components"), SerializeField, AssetsOnly, Required]
        [OnValueChanged(nameof(HandleItemChanged))]
        private Item _item;

        private Item _previousItem;

        [FoldoutGroup("Components"), ChildGameObjectsOnly, SerializeField, Required]
        private Transform _itemObject;

        [FoldoutGroup("Components"), ChildGameObjectsOnly, SerializeField, Required]
        [OnValueChanged(nameof(HandleItemChanged))]
        private Renderer _itemRenderer;

        [FoldoutGroup("Components"), ChildGameObjectsOnly, SerializeField, Required]
        private RectTransform _uiPanel;

        [FoldoutGroup("Components"), ChildGameObjectsOnly, SerializeField, Required]
        [OnValueChanged(nameof(HandleNameTMPChanged))]
        private TMP_Text _nameTMP;

        [SerializeField, HideInInspector]
        private TextEffect _nameEffect;

        [SerializeField, HideInInspector]
        private LocalizeStringEvent _nameLocalizeStringEvent;

        [FoldoutGroup("Components"), ChildGameObjectsOnly, SerializeField, Required]
        private TMP_Text _priceTMP;

        [FoldoutGroup("Animation"), SerializeField]
        private Vector3 _position;

        [FoldoutGroup("Animation"), SerializeField]
        private float _animationTime = 0.3f;

        [FoldoutGroup("Animation"), SerializeField]
        private AnimationCurve _animationEasing;

        [FoldoutGroup("Animation"), SerializeField]
        private float _rotationSpeed = 180;

        [FoldoutGroup("SFX"), SerializeField]
        private AudioItem _hoverAudioItem;

        [FoldoutGroup("SFX"), SerializeField]
        private AudioItem _unhoverAudioItem;

        private void Start()
        {
            if (!Application.isPlaying) return;
            
            GameManager.AudioManager.GetOrCreateItem(_hoverAudioItem);
            GameManager.AudioManager.GetOrCreateItem(_unhoverAudioItem);

            // Make the ui invisible
            _uiPanel.localScale = Vector3.zero;

            ItemManager.OnItemChanged += UpdatePrice;
            UpdatePrice();
        }

        private void OnEnable()
        {
            _nameLocalizeStringEvent = _nameTMP?.GetComponent<LocalizeStringEvent>();
            _nameEffect = _nameTMP?.GetComponent<TextEffect>();
            HandleItemChanged();
        }

        private void HandleNameTMPChanged()
        {
            _nameLocalizeStringEvent = _nameTMP?.GetComponent<LocalizeStringEvent>();
            if (_nameLocalizeStringEvent)
            {
                var ls = _item.LocalizedString;
                _nameLocalizeStringEvent.StringReference
                                        .SetReference(ls.TableReference, ls.TableEntryReference);
                ls.RefreshString();
                _nameLocalizeStringEvent.RefreshString();
            }

            _nameEffect = _nameTMP?.GetComponent<TextEffect>();
            if (_nameTMP) _nameTMP.text = _item?.Name;
            ApplyNameEffects();
        }

        private void HandleItemChanged()
        {
            if (_previousItem) _previousItem.OnPropertyChanged -= HandleItemChanged;
            if (_item) _item.OnPropertyChanged += HandleItemChanged;
            _previousItem = _item;
            HandleItemPropertyChanged();
        }

        private void HandleItemPropertyChanged()
        {
            if (_itemRenderer) _itemRenderer.GetComponent<Renderer>().material = _item?.Material;
            if (_nameTMP) _nameTMP.text = _item?.Name;
            if (_nameLocalizeStringEvent)
                _nameLocalizeStringEvent.StringReference = _item?.LocalizedString;
            if (_priceTMP) _priceTMP.text = $"₦{_item?.Price.ToString()}";
            ApplyNameEffects();
        }

        private void UpdatePrice()
        {
            _priceTMP.text = _item.Unlocked
                                 ? ItemManager.SelectedItem == _item ? "Selected" : "Unlocked"
                                 : $"₦{_item.Price}";
            _priceTMP.color = _item.Unlocked
                                  ? ItemManager.SelectedItem == _item ? Color.cyan : Color.white
                                  : _item.Price > ItemManager.Coins
                                      ? Color.red
                                      : Color.green;
        }

        private void ApplyNameEffects()
        {
            _nameEffect.globalEffects.Clear();
            if (!_item) return;
            _nameEffect.globalEffects.AddRange(_item.NameTextEffects);
            _nameEffect.Refresh();
        }

        public void Select()
        {
            ItemManager.SelectItem(_item);
        }

        [Button]
        public void Hover()
        {
            this.StopRoutine(ref _deselectRoutine);
            this.RestartRoutine(ref _rotateRoutine, RotateRoutine());
            this.RestartRoutine(ref _selectRoutine, HoverRoutine());

            if (!Application.isPlaying) return;
            GameManager.AudioManager.Play(_hoverAudioItem);
        }

        [Button]
        public void Unhover()
        {
            this.StopRoutine(ref _selectRoutine);
            this.StopRoutine(ref _rotateRoutine);
            this.RestartRoutine(ref _deselectRoutine, UnhoverRoutine());

            if (!Application.isPlaying) return;
            GameManager.AudioManager.Play(_unhoverAudioItem);
        }

        private Coroutine _selectRoutine;

        private IEnumerator HoverRoutine()
        {
            var startPos = _itemObject.localPosition;
            var uiStartPos = _uiPanel.localPosition;
            var uiStartScale = _uiPanel.localScale;

            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = _animationEasing.Evaluate(
                    Misc.Tween(ref lerpPos, _animationTime, unscaled: true));

                _itemObject.localPosition = Vector3.LerpUnclamped(startPos, _position, t);
                _uiPanel.localPosition = Vector3.LerpUnclamped(uiStartPos, _position, t);
                _uiPanel.localScale = Vector3.LerpUnclamped(uiStartScale, Vector3.one, t);

                yield return null;
            }

            _selectRoutine = null;
        }

        private Coroutine _deselectRoutine;

        private IEnumerator UnhoverRoutine()
        {
            var startPos = _itemObject.localPosition;
            var startRot = _itemObject.localRotation;
            var uiStartPos = _uiPanel.localPosition;
            var uiStartScale = _uiPanel.localScale;

            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = _animationEasing.Evaluate(
                    Misc.Tween(ref lerpPos, _animationTime, unscaled: true));

                _itemObject.localPosition = Vector3.LerpUnclamped(startPos, Vector3.zero, t);
                _itemObject.localRotation =
                    Quaternion.LerpUnclamped(startRot, Quaternion.identity, t);
                _uiPanel.localPosition = Vector3.LerpUnclamped(uiStartPos, Vector3.zero, t);
                _uiPanel.localScale = Vector3.LerpUnclamped(uiStartScale, Vector3.zero, t);

                yield return null;
            }

            _deselectRoutine = null;
        }

        private Coroutine _rotateRoutine;

        private IEnumerator RotateRoutine()
        {
            var rot = Vector3.up * _rotationSpeed;

            while (true)
            {
                _itemObject.Rotate(rot * Time.deltaTime);
                yield return null;
            }
        }
    }
}