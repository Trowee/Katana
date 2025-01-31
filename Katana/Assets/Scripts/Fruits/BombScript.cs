using System.Collections.Generic;
using Assets.Scripts.KatanaMovement;
using UnityEngine;

namespace Assets.Scripts.Fruits
{
    [RequireComponent(typeof(Rigidbody))]
    public class BombScript : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private LayerMask _playerMask;
        [SerializeField] private float _destroyForce = 25;
        [SerializeField] private float _radius = 10;
        
        [Header("Particles")]
        [SerializeField] private Transform _particles;
        [SerializeField] private List<ParticleSystem> _explosionParticles;
        private float _destroyParticlesAfter = 25;

        private void Reset()
        {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision col)
        {
            GetDestroyed();
        }

        public void GetDestroyed()
        {
            Kill();
            _collider.enabled = false;
            _explosionParticles.ForEach(x => x.Play());
            _particles.SetParent(null);
            Destroy(_particles.gameObject, _destroyParticlesAfter);
            Destroy(gameObject);
        }

        private void Kill()
        {
            var hits = Physics.OverlapSphere(transform.position, _radius, _playerMask);
            foreach (var hit in hits)
                if (hit.transform.parent.TryGetComponent(out PlayerMovementScript player)) player.Die();
        }
    }
}