using System.Collections;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using NnUtils.Scripts;
using NnUtils.Modules.Easings;
using UnityEngine;
using Assets.Scripts.Colosseum;

namespace Assets.Scripts.Fruits
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(Slice))]
    [RequireComponent(typeof(Fracture))]
    public class FruitScript : MonoBehaviour, IAimable, IFragmentable
    {
        [FoldoutGroup("Components")]
        [SerializeField, Required] private Collider _collider;
        [FoldoutGroup("Components")]
        [SerializeField, Required] private Rigidbody _rigidbody;
        [FoldoutGroup("Components")]
        [ValidateInput(nameof(RendererValidation))]
        [SerializeField] private Renderer _renderer;

        private bool RendererValidation(ref string msg)
        {
            if (!_renderer)
            {
                msg = "Fruit Renderer can't be null";
                return false;
            }
            var materials = _renderer.sharedMaterials;
            if (materials.Length < 2)
            {
                msg = "Fruit Renderer must have 2 materials, 2nd being the Outline";
                return false;
            }
            return true;
        }

        [FoldoutGroup("Spawning")]
        [SerializeField] private float _spawnAnimTime = 0.25f;
        [FoldoutGroup("Spawning")]
        [SerializeField] private AnimationCurve _spawnAnimCurve;

        [FoldoutGroup("Destruction")]
        [SerializeField, AssetsOnly] private FragmentScript _fragmentSettings;
        [FoldoutGroup("Destruction")]
        [SerializeField] private LayerMask _destructionMask;
        [FoldoutGroup("Destruction")]
        [SerializeField] private int _rewardCoins;

        [FoldoutGroup("Destruction/Slice")]
        [SerializeField, Required] private Slice _slice;
        [FoldoutGroup("Destruction/Fracture")]
        [SerializeField, Required] private Fracture _fracture;

        [FoldoutGroup("Outline")]
        [SerializeField] private float _outlineTransitionDuration;
        [FoldoutGroup("Outline")]
        [SerializeField] private Easings.Type _outlineTransitionEasing;

        [FoldoutGroup("Particles")]
        [SerializeField] private Transform _particles;
        [FoldoutGroup("Particles")]
        [SerializeField] private List<ParticleSystem> _explosionParticles;

        private Material _outlineMat;
        private float _outlineThickness;

        private void Reset()
        {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
            _renderer = GetComponent<Renderer>();
            _slice = GetComponent<Slice>();
            _fracture = GetComponent<Fracture>();
        }

        private void Awake()
        {
            _outlineMat = _renderer.materials[1];
            _outlineMat.SetFloat("_OutlineOpacity", 0);
            _outlineThickness = _outlineMat.GetFloat("_OutlineThickness");
            _outlineMat.SetFloat("_OutlineThickness", 1);
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

        private void SetParticleLifetime()
        {
            _explosionParticles.ForEach(x =>
                {
                    var main = x.main;
                    main.startLifetime = ColosseumSceneManager.FruitParticleLifetime;
                });
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

        public void AimAt()
        {
            if (this == null) return;
            this.RestartRoutine(ref _fadeOutlineRoutine,
                FadeOutlineRoutine(1, _outlineThickness));
        }

        public void AimAway()
        {
            if (this == null) return;
            this.RestartRoutine(ref _fadeOutlineRoutine, FadeOutlineRoutine());
        }

        private Coroutine _fadeOutlineRoutine;
        private IEnumerator FadeOutlineRoutine(float opacity = 0, float thickness = 1)
        {
            var startOpacity = _outlineMat.GetFloat("_OutlineOpacity");
            var startThickness = _outlineMat.GetFloat("_OutlineThickness");

            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = Misc.Tween(
                    ref lerpPos, _outlineTransitionDuration, _outlineTransitionEasing, true);
                _outlineMat.SetFloat("_OutlineOpacity",
                    Mathf.Lerp(startOpacity, opacity, t));
                _outlineMat.SetFloat("_OutlineThickness",
                    Mathf.Lerp(startThickness, thickness, t));
                yield return null;
            }

            _fadeOutlineRoutine = null;
        }


        public bool GetFractured(
            out List<FragmentScript> fragments,
            float? impactVelocity = null,
            GameObject sender = null)
        {
            fragments = new();
            if (impactVelocity != null &&
                impactVelocity < ColosseumSceneManager.DestructionVelocity) return false;

            if (sender == ColosseumSceneManager.Player.gameObject)
                GameManager.ItemManager.RewardCoins(_rewardCoins);

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
            if (impactVelocity != null &&
                impactVelocity < ColosseumSceneManager.DestructionVelocity) return false;

            if (sender == ColosseumSceneManager.Player.gameObject)
                GameManager.ItemManager.RewardCoins(_rewardCoins);

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
                frag.Rigidbody.linearVelocity = _rigidbody.linearVelocity;
            });

            Destroy(fragments[0].transform.parent.gameObject, _fragmentSettings.Lifetime + 1);
            return frags;
        }

        private void GetDestroyed()
        {
            this.StopRoutine(ref _spawnRoutine);
            _collider.enabled = false;
            _particles.SetParent(null);
            _explosionParticles.ForEach(x => x.Play());
            Destroy(_particles.gameObject, ColosseumSceneManager.FruitParticleLifetime + 0.1f);
            Destroy(gameObject);
        }
    }
}
