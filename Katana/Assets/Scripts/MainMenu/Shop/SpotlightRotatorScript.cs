using UnityEngine;

namespace Assets.Scripts.MainMenu.Shop
{
    [ExecuteAlways]
    public class SpotlightRotatorScript : MonoBehaviour
    {
        [SerializeField] private Vector3 _rotationSpeed = Vector3.forward * 20;
        
        private void Update() => transform.Rotate(_rotationSpeed * Time.deltaTime);
    }
}