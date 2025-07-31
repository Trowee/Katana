using UnityEngine;

namespace Assets.Scripts.Core
{
    [ExecuteAlways]
    public class ColosseumSun : MonoBehaviour
    {
        [SerializeField] private Vector3 _rotationSpeed = new(0, 2.5f, 0);
        private void Update() => transform.Rotate(_rotationSpeed * Time.deltaTime);
    }
}