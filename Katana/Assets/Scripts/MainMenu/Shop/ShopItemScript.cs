using System.Collections;
using NnUtils.Scripts;
using UnityEngine;

namespace Assets.Scripts.MainMenu.Shop
{
    public class ShopItemScript : MonoBehaviour
    {
        [Header("Components")]

        [Tooltip("Mesh that will be animated")]
        [SerializeField] private Transform _item;

        [Header("Animation")]

        [Tooltip("Position to which the object will be moved")]
        [SerializeField] private Vector3 _position;

        [Tooltip("Whether the position will be used as an offset or absolute position")]
        [SerializeField] private bool _offset;

        [Tooltip("How long the move transition will last")]
        [SerializeField] private float _animationTime = 1;

        [Tooltip("Curve used for the move transition")]
        [SerializeField] private AnimationCurve _animationEasing;

        [Tooltip("Speed at which the object will rotate")]
        [SerializeField] private float _rotationSpeed = 180;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A)) Select();
            if (Input.GetKeyDown(KeyCode.D)) Deselect();
        }

        private void OnMouseEnter()
        {
            Select();
        }

        private void OnMouseExit()
        {
            Deselect();
        }

        private void Select()
        {
            this.StopRoutine(ref _deselectRoutine);
            this.RestartRoutine(ref _rotateRoutine, RotateRoutine());
            this.RestartRoutine(ref _selectRoutine, SelectRoutine());
        }

        private void Deselect()
        {
            this.StopRoutine(ref _rotateRoutine);
            this.StopRoutine(ref _selectRoutine);
            this.RestartRoutine(ref _deselectRoutine, DeselectRoutine());
        }

        private Coroutine _selectRoutine;
        private IEnumerator SelectRoutine()
        {
            float lerpPos = 0;

            while (lerpPos < 1)
            {
                var t = _animationEasing.Evaluate(Misc.Tween(ref lerpPos, _animationTime, unscaled: true));
                _item.localPosition = Vector3.LerpUnclamped(Vector3.zero, _position, t);
                yield return null;
            }

            _selectRoutine = null;
        }

        private Coroutine _deselectRoutine;
        private IEnumerator DeselectRoutine()
        {
            var startPos = _item.localPosition;
            var startRot = _item.localRotation;
            float lerpPos = 0;

            while (lerpPos < 1)
            {
                var t = _animationEasing.Evaluate(Misc.Tween(ref lerpPos, _animationTime, unscaled: true));
                _item.localPosition = Vector3.LerpUnclamped(startPos, Vector3.zero, t);
                _item.localRotation = Quaternion.LerpUnclamped(startRot, Quaternion.identity, t);
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
                _item.Rotate(rot * Time.deltaTime);
                yield return null;
            }
        }
    }
}