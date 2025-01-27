using System.Collections;
using Assets.Scripts.Core;
using Assets.Scripts.Items;
using NnUtils.Scripts;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.MainMenu.Shop
{
    public class ShopItemScript : MonoBehaviour
    {
        [Header("Components")]
        
        [Tooltip("Item")]
        [SerializeField] private Item _item;

        [Tooltip("Mesh that will be animated")]
        [SerializeField] private Transform _itemTransform;

        [Tooltip("Panel that holds ui components")]
        [SerializeField] private RectTransform _uiPanel;

        [Tooltip("Name TMP_Text")]
        [SerializeField] private TMP_Text _nameTMP;

        [Header("Animation")]

        [Tooltip("Position to which the object will be moved")]
        [SerializeField] private Vector3 _position;

        [Tooltip("How long the move transition will last")]
        [SerializeField] private float _animationTime = 1;

        [Tooltip("Curve used for the move transition")]
        [SerializeField] private AnimationCurve _animationEasing;

        [Tooltip("Speed at which the object will rotate")]
        [SerializeField] private float _rotationSpeed = 180;

        private void Awake()
        {
            _nameTMP.text = _item.Name;
        }

        public void Select()
        {
            GameManager.ItemManager.SelectItem(_item);
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
            Vector3 startPos = _itemTransform.localPosition;
            Vector3 uiStartPos = _uiPanel.localPosition;
            Vector3 uiStartScale = _uiPanel.localScale;

            float lerpPos = 0;

            while (lerpPos < 1)
            {

                // Update T
                var t = _animationEasing.Evaluate(Misc.Tween(ref lerpPos, _animationTime, unscaled: true));

                // Update item
                _itemTransform.localPosition = Vector3.LerpUnclamped(startPos, _position, t);

                // Update UI
                _uiPanel.localPosition = Vector3.LerpUnclamped(uiStartPos, _position, t);
                _uiPanel.localScale = Vector3.LerpUnclamped(uiStartScale, Vector3.one, t);

                // Wait for the next frame
                yield return null;
            }

            _selectRoutine = null;
        }

        private Coroutine _deselectRoutine;
        private IEnumerator DeselectRoutine()
        {
            var startPos = _itemTransform.localPosition;
            var startRot = _itemTransform.localRotation;
            var uiStartPos = _uiPanel.localPosition;
            var uiStartScale = _uiPanel.localScale;

            float lerpPos = 0;

            while (lerpPos < 1)
            {
                // Update T
                var t = _animationEasing.Evaluate(Misc.Tween(ref lerpPos, _animationTime, unscaled: true));

                // Update Item
                _itemTransform.localPosition = Vector3.LerpUnclamped(startPos, Vector3.zero, t);
                _itemTransform.localRotation = Quaternion.LerpUnclamped(startRot, Quaternion.identity, t);

                // Update UI
                _uiPanel.localPosition = Vector3.LerpUnclamped(uiStartPos, Vector3.zero, t);
                _uiPanel.localScale = Vector3.LerpUnclamped(uiStartScale, Vector3.zero, t);

                // Wait for the next frame
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
                _itemTransform.Rotate(rot * Time.deltaTime);
                yield return null;
            }
        }
    }
}