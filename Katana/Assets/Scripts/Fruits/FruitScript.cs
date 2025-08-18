using System.Collections;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using NnUtils.Scripts;
using SadnessMonday.BetterPhysics;
using UnityEngine;

namespace Assets.Scripts.Fruits
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BetterRigidbody))]
    [RequireComponent(typeof(Slice))]
    [RequireComponent(typeof(Fracture))]
    public class FruitScript : MonoBehaviour, IFragmentable
    {
        [SerializeField, Required] private Collider _collider;
        [SerializeField, Required] private Rigidbody _rigidbody;
        [SerializeField] private int _coins;

        [FoldoutGroup("Spawning")]
        [SerializeField] private float _spawnAnimTime = 0.25f;
        [SerializeField] private AnimationCurve _spawnAnimCurve;

        [FoldoutGroup("Destruction")]
        [SerializeField, AssetsOnly] private FragmentScript _fragmentSettings;
        [FoldoutGroup("Destruction")]
        [SerializeField] private LayerMask _destructionMask;
        [FoldoutGroup("Destruction")]
        [SerializeField] private float _minDestructionVelocity = 50;
        [FoldoutGroup("Destruction")]
        [SerializeField] private int _fruitFragmentLayer = 5;

        [FoldoutGroup("Destruction/Slice")]
        [SerializeField, Required] private Slice _slice;
        [FoldoutGroup("Destruction/Fracture")]
        [SerializeField, Required] private Fracture _fracture;

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

        private void Start()
        {
            _spawnRoutine = StartCoroutine(SpawnRoutine());
        }

        private void OnCollisionEnter(Collision col)
        {
            if (col.gameObject != ColosseumSceneManager.Player.gameObject &&
                ((1 << col.gameObject.layer) & _destructionMask) != 0)
                GetFractured(out _);
        }

        private Coroutine _spawnRoutine;
        private IEnumerator SpawnRoutine()
        {
            var originalScale = transform.localScale;
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = _spawnAnimCurve.Evaluate(
                    Misc.Tween(ref lerpPos, _spawnAnimTime, unscaled: true));
                transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
                yield return null;
            }
        }

        public bool GetFractured(
            out List<FragmentScript> fragments,
            float? impactVelocity = null,
            GameObject sender = null)
        {
            fragments = new();
            if (impactVelocity != null && impactVelocity < _minDestructionVelocity) return false;

            if (sender == ColosseumSceneManager.Player.gameObject)
                GameManager.ItemManager.RewardCoins(_coins);

            fragments = HandleFragments(_fracture.ComputeFracture());
            GetDestroyed();

            return true;
        }

        public bool GetSliced(
            out List<FragmentScript> fragments,
            Vector3 sliceOrigin,
            Vector3 sliceNormal,
            float? impactVelocity = null,
            GameObject sender = null)
        {
            fragments = new();
            if (impactVelocity != null && impactVelocity < _minDestructionVelocity) return false;

            if (sender == ColosseumSceneManager.Player.gameObject)
                GameManager.ItemManager.RewardCoins(_coins);

            fragments = HandleFragments(
                _slice.ComputeSlice(sliceNormal, sliceOrigin));
            GetDestroyed();

            return true;
        }

        private List<FragmentScript> HandleFragments(List<GameObject> fragments)
        {
            if (fragments.Count < 1) return new();

            List<FragmentScript> frags = new();
            fragments.ForEach(fragment =>
            {
                var frag = fragment.AddComponent<FragmentScript>();
                frag.CopySettings(_fragmentSettings);
                frag.GetDestroyed();
                frags.Add(frag);

                var brb = fragment.AddComponent<BetterRigidbody>();
                brb.PhysicsLayer = _fruitFragmentLayer;

                frag.Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
                frag.Rigidbody.linearVelocity = _rigidbody.linearVelocity;
            });

            Destroy(fragments[0].transform.parent.gameObject, _fragmentSettings.Lifetime + 1);
            return frags;
        }

        private void GetDestroyed()
        {
            StopCoroutine(_spawnRoutine);
            _collider.enabled = false;
            _particles.SetParent(null);
            _explosionParticles.ForEach(x => x.Play());
            Destroy(_particles.gameObject, DestroyParticlesAfter);
            Destroy(gameObject);
        }
    }
}
