using System.Collections;
using ArtificeToolkit.Attributes;
using NnUtils.Modules.Easings;
using NnUtils.Scripts;
using UnityEngine;

namespace Assets.Scripts.MainMenu
{
    public class MainMenuButton : MonoBehaviour
    {
        [SerializeField, Required] private Transform _mesh;
        [SerializeField] private Vector3 _selectedPosition = Vector3.zero;
        [SerializeField] private Vector3 _deselectedPosition = Vector3.left * 0.5f;
        [SerializeField] private float _animationTime = 1;
        [SerializeField] private EasingType _animationEasing = EasingType.ExpoOut;

        private void Reset()
        {
            if (transform.childCount > 0)
                _mesh = transform.GetChild(0);
        }

        [Button]
        public void Select()
        {
            this.RestartRoutine(ref _moveRoutine, MoveRoutine(_selectedPosition));
        }

        [Button]
        public void Deselect()
        {
            this.RestartRoutine(ref _moveRoutine, MoveRoutine(_deselectedPosition));
        }

        private Coroutine _moveRoutine;
        private IEnumerator MoveRoutine(Vector3 targetPos)
        {
            var startPos = _mesh.localPosition;

            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = Misc.Tween(ref lerpPos, _animationTime, _animationEasing, unscaled: true);
                _mesh.transform.localPosition = Vector3.LerpUnclamped(startPos, targetPos, t);
                yield return null;
            }
        }
    }
}
