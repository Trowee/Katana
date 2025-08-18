using System.Collections.Generic;
using System.Linq;
using ArtificeToolkit.Attributes;
using Assets.Scripts.Core;
using Assets.Scripts.Player.Camera;
using Assets.Scripts.TimeScale;
using NnUtils.Modules.Easings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerScript : MonoBehaviour
    {
        private static SettingsManager Settings => GameManager.SettingsManager;

        private InputAction _dashAction, _dodgeAction, _flipAction, _perspectiveAction;

        [FoldoutGroup("Dev")]
        [ReadOnly]
        public float ComboDuration;
        [FoldoutGroup("Dev")]
        [ReadOnly]
        public float TimeSinceLastAction;
        [FoldoutGroup("Dev")]
        [ReadOnly]
        public PlayerAction LastAction;

        #region Components

        [ValidateInput(nameof(RigidbodyValidation))]
        [FoldoutGroup("Components"), SerializeField, Required] private Rigidbody _rb;

        private bool RigidbodyValidation(ref string msg, ref LogType lt)
        {
            if (!_rb)
            {
                msg = "Rigidbody can't be null";
                lt = LogType.Error;
                return false;
            }
            if (_rb.collisionDetectionMode != CollisionDetectionMode.ContinuousDynamic)
            {
                msg = "Rigidbody Collision Detection Mode should be set to ContinuousDynamic, expect weird behaviour.";
                lt = LogType.Warning;
                return false;
            }
            return true;
        }

        [FoldoutGroup("Components"), SerializeField] private Collider _collider;
        [FoldoutGroup("Components"), SerializeField] private GameObject _katanaObject;
        [FoldoutGroup("Components"), SerializeField] private MeshFilter _mesh;
        [FoldoutGroup("Components"), SerializeField] private Renderer _renderer;
        [FoldoutGroup("Components"), SerializeField] private Transform _decalPrefab;
        [FoldoutGroup("Components"), SerializeField] private Transform _decalPoint;
        [FoldoutGroup("Components"), SerializeField] private LayerMask _sliceLayer;

        #endregion

        [FoldoutGroup("Camera"), SerializeField] private float _cameraSwitchDuration = 1;
        [FoldoutGroup("Camera"), SerializeField] private Easings.Type _cameraSwitchEasing = Easings.Type.ExpoOut;

        #region Movement

        [FoldoutGroup("Movement/Tilt"), SerializeField]
        private float _tiltSpeed = 1;
        [FoldoutGroup("Movement/Tilt"), SerializeField]
        [Tooltip("% of the angular velocity that remains after 1 second of tilting")]
        private float _tiltRotationMultiplier = 0.025f;

        [FoldoutGroup("Movement/Dash"), SerializeField]
        private Vector3 _dashForce = new(0, 800, 0);
        [FoldoutGroup("Movement/Dash"), SerializeField]
        private TimeScaleKeys _dashTimeScale;

        [FoldoutGroup("Movement/Dodge"), SerializeField]
        private Vector3 _dodgeForce = new(0, -300, 0);
        [FoldoutGroup("Movement/Dodge"), SerializeField]
        private TimeScaleKeys _dodgeTimeScale;

        [FoldoutGroup("Movement/Flip"), SerializeField]
        private Vector3 _flipForce = new(0, 500, 20);
        [FoldoutGroup("Movement/Flip"), SerializeField]
        private Vector3 _flipStuckForce = new(0, -100, -100);
        [FoldoutGroup("Movement/Flip"), SerializeField]
        private float _flipRotation = 180;
        [FoldoutGroup("Movement/Flip"), SerializeField]
        private float _flipRewardMultiplier = 2;
        [FoldoutGroup("Movement/Flip"), SerializeField]
        private float _flipRewardMultiplierTime = 2;
        [FoldoutGroup("Movement/Flip"), SerializeField]
        private TimeScaleKeys _flipTimeScale;

        [FoldoutGroup("Movement/BulletTime"), SerializeField]
        private TimeScaleKeys _bulletTimeScale;

        [FoldoutGroup("Movement/Stick"), SerializeField] private LayerMask _stickMask;
        [FoldoutGroup("Movement/Stick"), SerializeField] private float _minStickVelocity = 5;
        [FoldoutGroup("Movement/Stick"), SerializeField] private float _maxStickAngle = 45;

        #endregion

        [FoldoutGroup("Death"), SerializeField] private TimeScaleKeys _deathTimeScale;

        [FoldoutGroup("Dev"), ReadOnly] public bool IsStuck;

        private void Reset()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            InitializeInputs();
        }

        private void OnDisable()
        {
            UninitializeInputs();
        }

        private void Start()
        {
            _renderer.sharedMaterial = GameManager.ItemManager.SelectedItem.Material;
            ColosseumSceneManager.CameraManager.SwitchCameraHandler(
                Settings.Perspective, _cameraSwitchDuration,
                _cameraSwitchEasing, unscaled: true);
            GetStuck(null);
        }

        private void Update()
        {
            Tilt();
        }

        private void OnCollisionEnter(Collision col)
        {
            if (ColosseumSceneManager.IsDead) return;

            if (col.gameObject.TryGetComponent(out IFragmentable fragmentable))
                DestroyFragmentable(fragmentable, col);

            // Store the col layer
            var layer = col.gameObject.layer;

            // Try to stick
            if ((_stickMask & 1 << layer) != 0) Stick(col);
        }

        private void InitializeInputs()
        {
            var inputActionAsset = GameManager.InputActionAsset;

            _dashAction = inputActionAsset.FindAction("Dash");
            _dashAction.performed += OnDash;

            _dodgeAction = inputActionAsset.FindAction("Dodge");
            _dodgeAction.performed += OnDodge;

            _flipAction = inputActionAsset.FindAction("Flip");
            _flipAction.performed += OnFlip;

            _perspectiveAction = inputActionAsset.FindAction("Perspective");
            _perspectiveAction.performed += OnPerspective;
        }

        private void UninitializeInputs()
        {
            _dashAction.performed -= OnDash;
            _dodgeAction.performed -= OnDodge;
            _flipAction.performed -= OnFlip;
            _perspectiveAction.performed -= OnPerspective;
        }

        private void SpawnDecal(Transform parent)
        {
            var decal = Instantiate(_decalPrefab, _decalPoint);
            decal.SetParent(parent);
        }

        private void Tilt()
        {
            if (ColosseumSceneManager.IsDead) return;

            // Return if stuck
            if (IsStuck) return;

            // Store mouse input
            var x = Input.GetAxisRaw("Mouse X") * _tiltSpeed;
            var y = Input.GetAxisRaw("Mouse Y") * _tiltSpeed;

            // Return if there is no input
            if (x == 0 && y == 0) return;

            // Drastically lower the rotation when panning
            _rb.angularVelocity *= Mathf.Pow(_tiltRotationMultiplier, Time.deltaTime);

            // Apply the rotation
            var deltaRotX = Quaternion.AngleAxis(x, Vector3.back);
            var deltaRotY = Quaternion.AngleAxis(y, Vector3.left);
            var deltaRot = deltaRotX * deltaRotY;
            _rb.MoveRotation(_rb.rotation * deltaRot);
        }

        private bool ApplyPlayerFlippedMultiplier =>
        ColosseumSceneManager.IsDead &&
            LastAction == PlayerAction.Flip &&
            TimeSinceLastAction <= 2;

        private void OnDash(InputAction.CallbackContext ctx) => Dash();
        private void OnDodge(InputAction.CallbackContext ctx) => Dodge();
        private void OnFlip(InputAction.CallbackContext ctx) => Flip();
        private void OnPerspective(InputAction.CallbackContext ctx) => ChangePerspective();

        private void Dash()
        {
            if (ColosseumSceneManager.IsDead) return;

            LastAction = PlayerAction.Dash;
            GetUnstuck();

            // Reset velocity
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            // Add force
            _rb.AddRelativeForce(_dashForce, ForceMode.VelocityChange);

            // Apply timescale changes
            GameManager.TimeScaleManager.UpdateTimeScale(_dashTimeScale);
        }

        private void Dodge()
        {
            if (ColosseumSceneManager.IsDead) return;

            LastAction = PlayerAction.Dodge;
            GetUnstuck();

            // Reset velocity
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;

            // Add force
            _rb.AddRelativeForce(_dodgeForce, ForceMode.VelocityChange);

            // Apply timescale changes
            GameManager.TimeScaleManager.UpdateTimeScale(_dodgeTimeScale);
        }

        private void Flip()
        {
            if (ColosseumSceneManager.IsDead) return;

            LastAction = PlayerAction.Flip;
            var wasStuck = IsStuck;
            GetUnstuck();

            // Add force and rotation
            _rb.AddRelativeForce(wasStuck ? _flipStuckForce : _flipForce, ForceMode.VelocityChange);
            _rb.AddRelativeTorque(new(_flipRotation, 0), ForceMode.Impulse);

            // Apply timescale changes
            GameManager.TimeScaleManager.UpdateTimeScale(_flipTimeScale);
        }

        private void ChangePerspective()
        {
            if (ColosseumSceneManager.IsDead) return;

            Settings.Perspective = Settings.Perspective switch
            {
                Perspective.First => Perspective.Right,
                Perspective.Right => Perspective.Third,
                Perspective.Third => Perspective.Left,
                _ => Perspective.First
            };

            ColosseumSceneManager.CameraManager.SwitchCameraHandler(Settings.Perspective, _cameraSwitchDuration, _cameraSwitchEasing, unscaled: true);
        }

        private void EnterBulletTime() =>
            GameManager.TimeScaleManager.UpdateTimeScale(_bulletTimeScale);

        private void Stick(Collision col)
        {
            if (IsStuck) return;
            if (col.relativeVelocity.magnitude < _minStickVelocity) return;

            // Check if any of the normals are at a low enough angle relative to the player
            var fw = transform.up;
            var threshold = -Mathf.Cos(_maxStickAngle * Mathf.Deg2Rad);
            if (col.contacts.Select(contact => contact.normal)
                .Select(contactNormal => Vector3.Dot(fw, contactNormal))
                .Any(dp => !(dp > threshold)))
                GetStuck(col.transform);
        }

        private void GetStuck(Transform parent)
        {
            // Freeze rb
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;

            // Set parent to follow that object
            transform.SetParent(parent);
            SpawnDecal(parent);

            // Set IsStuck to true
            IsStuck = true;
        }

        private void GetUnstuck()
        {
            if (!IsStuck) return;
            _rb.isKinematic = false;
            transform.SetParent(null);
            IsStuck = false;
        }

        private void DestroyFragmentable(IFragmentable fragmentable, Collision col)
        {
            foreach (var c in col.contacts)
            {
                var cPos = c.point;
                var cNorm = c.normal;

                Vector3[] axes = {
                    transform.forward,
                    -transform.forward,
                    transform.up,
                    -transform.up,
                    transform.right,
                    -transform.right
                };

                // Find the axis most aligned with the collision normal
                float bestDot = -1f;
                int bestAxis = -1;
                for (int i = 0; i < axes.Length; i++)
                {
                    float dot = Vector3.Dot(axes[i], -cNorm);
                    if (dot > bestDot)
                    {
                        bestDot = dot;
                        bestAxis = i;
                    }
                }

                Vector3? sliceDir = null;

                switch (bestAxis)
                {
                    case 0: // Forward (Blade)
                        sliceDir = Vector3.Cross(transform.up, transform.forward)
                        .normalized;
                        break;
                    case 1: // Back (Blade Back)
                        sliceDir = Vector3.Cross(transform.up, -transform.forward)
                        .normalized;
                        break;
                    case 2: // Up (Tip)
                        sliceDir = Vector3.Cross(transform.forward, transform.up)
                        .normalized;
                        break;
                }

                List<FragmentScript> fragments;
                if (sliceDir.HasValue
                    ? !fragmentable.GetSliced(
                        out fragments,
                        cPos, transform.right, _rb.linearVelocity.magnitude, gameObject)
                    : !fragmentable.GetFractured(
                        out fragments,
                        _rb.linearVelocity.magnitude, gameObject)) continue;

                // Check if player died in case it was a bomb
                if (ColosseumSceneManager.IsDead) return;

                var distances = new float[fragments.Count];
                var maxDistance = 0f;
                for (int i = 0; i < fragments.Count; i++)
                {
                    // We have to use the renderer since origin is at the center of fracture
                    var fragPos = fragments[i].GetComponent<Renderer>()
                        .bounds.center /*- fragPosDelta*/;
                    distances[i] = Vector3.Distance(cPos, fragPos);
                    if (distances[i] > maxDistance)
                        maxDistance = distances[i];
                }
                for (int i = 0; i < fragments.Count; i++)
                {
                    var rb = fragments[i].Rigidbody;
                    var t = distances[i] / maxDistance;
                    var vel = sliceDir.HasValue
                        ? Vector3.zero
                        : Vector3.Lerp(Vector3.zero, _rb.linearVelocity * 0.75f, t);
                    fragments[i].Rigidbody.linearVelocity = vel;
                }

                // Might implement this stopping when I add forward detection for collat idk yet
                _rb.linearVelocity *= 0.75f;
                _rb.angularVelocity *= 0.75f;
                EnterBulletTime();

                break;
            }
        }

        public void Die()
        {
            StopAllCoroutines();
            GameManager.TimeScaleManager.UpdateTimeScale(_deathTimeScale, 1000);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _katanaObject.SetActive(false);
            _rb.isKinematic = true;
            _collider.enabled = false;
            UninitializeInputs();
            ColosseumSceneManager.Die();
        }
    }
}
