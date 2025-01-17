using UnityEngine;

namespace KatanaMovement
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform _katana;
        [SerializeField] private Vector3 _offset = new(0, 3, -5f);

        private void Update()
        {
            var pos = transform.position;
            transform.position = _katana.transform.position;

            var rot = transform.rotation.eulerAngles;
            var katanaRot = _katana.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
            
            transform.Translate(_offset);
        }
    }
}