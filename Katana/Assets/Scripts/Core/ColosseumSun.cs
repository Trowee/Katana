using UnityEngine;

namespace Assets.Scripts.Core
{
    [ExecuteAlways]
    public class ColosseumSun : MonoBehaviour
    {
        [SerializeField] private Vector2 _tiltRange = new(-25, 30);
        [SerializeField] private Vector2 _rotationSpeed = new(1, 2.5f);
        private int _tiltDir = 1;
        
        private void Update()
        {
            var xRot = transform.eulerAngles.x;
            if (xRot >= _tiltRange.x &&
                xRot <= _tiltRange.y) _tiltDir *= -1;
            
            var rot = new Vector3(_rotationSpeed.x * _tiltDir, _rotationSpeed.y) * Time.deltaTime;
            transform.localEulerAngles += rot;
        }
    }
}