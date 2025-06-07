using System;
using System.Collections;
using System.Globalization;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using Assets.Scripts.Items;
using EasyTextEffects;
using NnUtils.Scripts;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Assets.Scripts.MainMenu.Shop
{
    // TODO: Add ChildGameObjectsOnly attribute to fields when fixed
    [ExecuteAlways]
    public class ShopItemScript : MonoBehaviour
    {
        private static ItemManager ItemManager => GameManager.ItemManager;

        [FoldoutGroup("Components"), SerializeField, AssetsOnly, Required]
        [OnValueChanged(nameof(HandleItemChanged))]
        private Item _item;

        private Item _previousItem;

        [FoldoutGroup("Components"), SerializeField, Required]
        private Transform _itemObject;

        [FoldoutGroup("Components"), SerializeField, Required]
        [OnValueChanged(nameof(HandleItemChanged))]
        private Renderer _itemRenderer;

        [FoldoutGroup("Components"), SerializeField, Required]
        private RectTransform _uiPanel;

        [FoldoutGroup("Components"), SerializeField, Required]
        [OnValueChanged(nameof(HandleNameChanged))]
        private TMP_Text _nameTMP;
        
        [SerializeField, HideInInspector]
        private TextEffect _nameEffect;

        private void HandleNameChanged()
        {
            _nameEffect = _nameTMP?.GetComponent<TextEffect>();
            if (_nameTMP) _nameTMP.text = _item?.Name;
            ApplyNameEffects();
        }

        [FoldoutGroup("Components"), SerializeField, Required]
        private TMP_Text _priceTMP;

        [FoldoutGroup("Animation"), SerializeField]
        private Vector3 _position;

        [FoldoutGroup("Animation"), SerializeField]
        private float _animationTime = 0.3f;

        [FoldoutGroup("Animation"), SerializeField]
        private AnimationCurve _animationEasing;

        [FoldoutGroup("Animation"), SerializeField]
        private float _rotationSpeed = 180;

        private void Start()
        {
            if (Application.isEditor) return;
            
            // Make the ui invisible
            _uiPanel.localScale = Vector3.zero;

            ItemManager.OnItemChanged += UpdatePrice;
            UpdatePrice();
        }

        private void OnEnable()
        {
            HandleItemChanged();
        }

        private void HandleItemChanged()
        {
            if (_previousItem) _previousItem.OnPropertyChanged -= HandleItemChanged;
            if (_item) _item.OnPropertyChanged += HandleItemChanged;
            _previousItem = _item;
            if (_item) HandleItemPropertyChanged();
        }

        private void HandleItemPropertyChanged()
        {
            if (_itemRenderer) _itemRenderer.GetComponent<Renderer>().material = _item?.Material;
            if (_nameTMP) _nameTMP.text = _item?.Name;
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
            this.RestartRoutine(ref _selectRoutine, SelectRoutine());
        }

        [Button]
        public void Unhover()
        {
            this.StopRoutine(ref _rotateRoutine);
            this.StopRoutine(ref _selectRoutine);
            this.RestartRoutine(ref _deselectRoutine, DeselectRoutine());
        }

        private Coroutine _selectRoutine;

        private IEnumerator SelectRoutine()
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

        private IEnumerator DeselectRoutine()
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