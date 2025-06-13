using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Fruits
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Fracture))]
    public class BombScript : MonoBehaviour, IDestructible
    {
        [SerializeField, Required] private Collider _collider;
        [SerializeField, Required] private Rigidbody _rigidbody;
        [SerializeField] private float _destroyForce = 25;

        [FoldoutGroup("Destruction")]
        [SerializeField, Required] private Fracture _fracture;
        [FoldoutGroup("Destruction")]
        [SerializeField, Required, AssetsOnly] private FragmentScript _fragmentSettings;
        [FoldoutGroup("Destruction/Explosion")]
        [SerializeField] private LayerMask _destructibleMask;
        [FoldoutGroup("Destruction/Explosion")]
        [SerializeField] private float _explosionRadius = 10;
        [FoldoutGroup("Destruction/Explosion")]
        [SerializeField] private float _explosionForce = 50;

        private void Reset()
        {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            _fracture = GetComponent<Fracture>();
        }

        private void OnCollisionEnter(Collision col) => Explode();

        [Button]
        public void Explode()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("This function can only be ran in Play Mode");
                return;
            }

            GetFractured(transform.position, _explosionForce);
            DestroyEntities();
            AffectRigidbodies();
            GetDestroyed();
        }

        public void GetFractured(Vector3 forceOrigin = default, float fractureForce = 0)
            => HandleFragments(_fracture.ComputeFracture());

        public void GetSliced(Vector3 o = default, Vector3 n = default, float v = 0) => Explode();

        private void HandleFragments(List<GameObject> fragments)
        {
            fragments.ForEach(fragment =>
            {
                var frag = fragment.AddComponent<FragmentScript>();
                frag.CopySettings(_fragmentSettings);
                frag.GetDestroyed();
            });
        }

        private void GetDestroyed()
        {
            _collider.enabled = false;
            ColosseumSceneManager.ExplosionManager.PlayExplosion(transform.position);
            Destroy(gameObject);
        }

        private void DestroyEntities()
        {
            var hits = Physics.OverlapSphere(
                transform.position, _explosionRadius, _destructibleMask);

            foreach (var hit in hits)
                if (hit.TryGetComponent(out IDestructible destructible))
                    destructible.GetFractured(transform.position);
                else if (hit.transform.parent != null &&
                         hit.transform.parent.TryGetComponent(out PlayerScript player))
                    player.Die();
        }

        private void AffectRigidbodies()
        {
            var hits = Physics.OverlapSphere(
                transform.position, _explosionRadius, _destructibleMask);

            foreach (var hit in hits)
                if (hit.TryGetComponent(out Rigidbody rb))
                    rb.AddExplosionForce(_explosionForce, transform.position, _explosionRadius, 0,
                                         ForceMode.Impulse);
        }
    }
}