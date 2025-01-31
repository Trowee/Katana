using System.Collections;
using NnUtils.Scripts;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class ScoreScript : MonoBehaviour
    {
        [SerializeField] private TMP_Text _score;
        [SerializeField] private RectTransform _rt;
        [SerializeField] private float _animTime = 1;
        [SerializeField] private AnimationCurve _animCurve;

        private void Start()
        {
            UpdateScore();
            GameManager.ItemManager.OnCoinsChanged += UpdateScore;
        }

        private void OnDestroy() => GameManager.ItemManager.OnCoinsChanged -= UpdateScore;
        
        private void UpdateScore()
        {
            _score.text = $@"₦{GameManager.ItemManager.Coins.ToString()}";
            this.RestartRoutine(ref _updateScoreRoutine, UpdateScoreRoutine());
        }

        private Coroutine _updateScoreRoutine;
        private IEnumerator UpdateScoreRoutine()
        {
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = _animCurve.Evaluate(Misc.Tween(ref lerpPos, _animTime, unscaled: true));
                _rt.localScale = Vector3.one * t;
                yield return null;
            }
            _updateScoreRoutine = null;
        }
    }
}