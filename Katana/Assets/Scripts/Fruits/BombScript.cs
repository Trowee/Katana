using System.Collections.Generic;
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
            var layer = col.gameObject.layer;
            var player = (_playerMask & 1 << layer) != 0;
            if (col.relativeVelocity.magnitude >= _destroyForce) GetDestroyed();
        }

        public void GetDestroyed()
        {
            _collider.enabled = false;
            _explosionParticles.ForEach(x => x.Play());
            _particles.SetParent(null);
            Destroy(_particles.gameObject, _destroyParticlesAfter);
            Destroy(gameObject);
        }
    }
}