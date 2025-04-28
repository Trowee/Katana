using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Fruits
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class FruitScript : MonoBehaviour, ISliceable
    {
        [SerializeField, ReadOnly] private Collider _collider;
        [SerializeField, ReadOnly] private Rigidbody _rigidbody;
        [SerializeField] private Slice _slice;
        [SerializeField] private LayerMask _pointsMask;
        [SerializeField] private float _destroyForce = 25;
        [SerializeField] private int _coins;
        
        [Header("Particles")]
        [SerializeField] private Transform _particles;
        [SerializeField] private List<ParticleSystem> _explosionParticles;
        private const float DestroyParticlesAfter = 11;

        private void Reset()
        {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            _slice = GetComponent<Slice>();
        }

        public void GetSliced(Vector3 sliceOrigin, Vector3 sliceNormal, float sliceVelocity)
        {
            if (sliceVelocity < _destroyForce) return;
            _slice.ComputeSlice(sliceNormal, sliceOrigin);
            GetDestroyed(true);
        }

        private void OnCollisionEnter(Collision col)
        {
            if ((_pointsMask & 1 << col.gameObject.layer) == 0) return;
            GetDestroyed(false);
        }

        public void GetDestroyed(bool destroyedByPlayer)
        {
            if (destroyedByPlayer) GameManager.ItemManager.Coins += _coins;
            _collider.enabled = false;
            _explosionParticles.ForEach(x => x.Play());
            _particles.SetParent(null);
            Destroy(_particles.gameObject, DestroyParticlesAfter);
            Destroy(gameObject);
        }
    }
}