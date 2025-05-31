using System.Collections;
using System.Globalization;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using Assets.Scripts.Items;
using NnUtils.Scripts;
using TMPro;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Assets.Scripts.MainMenu.Shop
{
    public class ShopItemScript : MonoBehaviour
    {
        private static ItemManager ItemManager => GameManager.ItemManager;
        
        [FoldoutGroup("Components"), SerializeField, Required]
        private Item _item;

        [FoldoutGroup("Components"), SerializeField, Required]
        private Transform _itemObject;

        [FoldoutGroup("Components"), SerializeField, Required]
        private RectTransform _uiPanel;

        [FoldoutGroup("Components"), SerializeField, Required]
        private TMP_Text _nameTMP;
        
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
            _nameTMP.text    =  _item.Name;
            ItemManager.OnItemChanged += UpdateUI;
            UpdateUI();
        }

        private void UpdateUI()
        {
            _priceTMP.text = _item.Unlocked
                ? ItemManager.SelectedItem == _item ? "Selected" : "Unlocked"
                : $@"₦{_item.Price.ToString(CultureInfo.InvariantCulture)}";
            _priceTMP.color = _item.Unlocked
                ? ItemManager.SelectedItem == _item ? Color.cyan : Color.white
                : _item.Price > ItemManager.Coins ? Color.red : Color.green;
        }

        public void Select()
        {
            ItemManager.SelectItem(_item);
        }

        public void Hover()
        {
            this.StopRoutine(ref _deselectRoutine);
            this.RestartRoutine(ref _rotateRoutine, RotateRoutine());
            this.RestartRoutine(ref _selectRoutine, SelectRoutine());
        }

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
                _uiPanel.localPosition  = Vector3.LerpUnclamped(uiStartPos, _position, t);
                _uiPanel.localScale     = Vector3.LerpUnclamped(uiStartScale, Vector3.one, t);

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
                _itemObject.localRotation = Quaternion.LerpUnclamped(startRot, Quaternion.identity, t);
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