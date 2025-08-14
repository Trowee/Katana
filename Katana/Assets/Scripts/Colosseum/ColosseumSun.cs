using UnityEngine;

namespace Assets.Scripts.Colosseum
{
    [ExecuteAlways]
    public class ColosseumSun : MonoBehaviour
    {
        [SerializeField] private Vector2 _tiltRange = new(-60, 60);
        [SerializeField] private float _tiltCycleTime = 237.31f;
        [SerializeField] private float _spinTime = 576.73f;

        private float _timeOffset;

        private void Start() => _timeOffset = Random.Range(0f, 100000f);

        void Update()
        {
            var t = (Mathf.Sin((Time.time + _timeOffset) / _tiltCycleTime) + 1f) / 2f;
            var xRot = Mathf.Lerp(_tiltRange.x, _tiltRange.y, t);
            var yRot = (Time.time + _timeOffset) % _spinTime / _spinTime * 360f;
            transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        }
    }
}
