using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using UnityEngine;

namespace Assets.Scripts.Fruits
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Slice))]
    [RequireComponent(typeof(Fracture))]
    public class FruitScript : MonoBehaviour, IDestructible
    {
        [SerializeField, Required] private Collider _collider;
        [SerializeField, Required] private Rigidbody _rigidbody;
        [SerializeField] private float _destroyForce = 100;
        [SerializeField] private int _coins;

        [FoldoutGroup("Destruction")]
        [SerializeField, AssetsOnly] FragmentScript _fragmentSettings;
        [FoldoutGroup("Destruction/Slice")]
        [SerializeField, Required] private Slice _slice;
        [FoldoutGroup("Destruction/Slice")]
        [SerializeField] private float _sliceForce = 20;
        [FoldoutGroup("Destruction/Fracture")]
        [SerializeField, Required] private Fracture _fracture;
        [FoldoutGroup("Destruction/Fracture")]
        [SerializeField] private float _fractureForce = 5;

        [FoldoutGroup("Particles")]
        [SerializeField] private Transform _particles;
        [FoldoutGroup("Particles")]
        [SerializeField] private List<ParticleSystem> _explosionParticles;
        private const float DestroyParticlesAfter = 11;

        private void Reset()
        {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            _slice = GetComponent<Slice>();
            _fracture = GetComponent<Fracture>();
        }

        private void OnCollisionEnter(Collision col)
        {
            if (col.gameObject != ColosseumSceneManager.Player.gameObject)
                GetFractured(transform.position, _fractureForce);
        }

        public void GetFractured(
            Vector3? forceOrigin = null,
            float fractureForce = 0,
            float impactVelocity = -1,
            GameObject sender = null)
        {
            if (impactVelocity != -1 && impactVelocity < _destroyForce) return;

            if (sender == ColosseumSceneManager.Player.gameObject)
                GameManager.ItemManager.RewardCoins(_coins);

            forceOrigin ??= transform.position;
            HandleFragments(_fracture.ComputeFracture(), (Vector3)forceOrigin, fractureForce);
            GetDestroyed();
        }

        public void GetSliced(
            Vector3 sliceOrigin,
            Vector3 sliceNormal,
            float impactVelocity = -1,
            GameObject sender = null)
        {
            if (impactVelocity != -1 && impactVelocity < _destroyForce) return;

            if (sender == ColosseumSceneManager.Player.gameObject)
                GameManager.ItemManager.RewardCoins(_coins);

            HandleFragments(_slice.ComputeSlice(sliceNormal, sliceOrigin),
                sliceOrigin, _sliceForce);
            GetDestroyed();
        }

        private void HandleFragments(List<GameObject> fragments,
                                     Vector3 forcePos = default,
                                     float force = 0)
        {
            if (fragments.Count < 1) return;

            fragments.ForEach(fragment =>
            {
                var frag = fragment.AddComponent<FragmentScript>();
                frag.CopySettings(_fragmentSettings);
                frag.GetDestroyed();

                var forceDir = fragment.transform.position - forcePos;
                frag.Rigidbody.linearVelocity = _rigidbody.linearVelocity;
                frag.Rigidbody.AddForce(force * forceDir, ForceMode.Impulse);
            });

            Destroy(fragments[0].transform.parent.gameObject, _fragmentSettings.Lifetime + 1);
        }

        private void GetDestroyed()
        {
            _collider.enabled = false;
            _particles.SetParent(null);
            _explosionParticles.ForEach(x => x.Play());
            Destroy(_particles.gameObject, DestroyParticlesAfter);
            Destroy(gameObject);
        }
    }
}
