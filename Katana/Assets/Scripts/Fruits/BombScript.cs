using Alchemy.Inspector;
using Assets.Scripts.Core;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Fruits
{
    [RequireComponent(typeof(Rigidbody))]
    public class BombScript : MonoBehaviour
    {
        [SerializeField, Required] private Collider _collider;
        [SerializeField] private LayerMask _playerMask;
        [SerializeField] private float _destroyForce = 25;
        [SerializeField] private float _radius = 10;

        private void Reset()
        {
            _collider = GetComponent<Collider>();
            GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision col)
        {
            GetDestroyed();
        }

        public void GetDestroyed()
        {
            Kill();
            _collider.enabled = false;
            PlaySceneManager.ExplosionManager.PlayExplosion(transform.position);
            Destroy(gameObject);
        }

        private void Kill()
        {
            var hits = Physics.OverlapSphere(transform.position, _radius, _playerMask);
            foreach (var hit in hits)
                if (hit.transform.parent.TryGetComponent(out PlayerScript player)) player.Die();
        }
    }
}