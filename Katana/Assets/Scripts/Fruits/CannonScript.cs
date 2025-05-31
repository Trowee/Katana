using System.Collections;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using NnUtils.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fruits
{
    public class CannonScript : MonoBehaviour
    {
        [FoldoutGroup("Cannon Parts"), SerializeField, Required]
        private Transform _cannon;

        [FoldoutGroup("Cannon Parts"), SerializeField, Required]
        private Transform _barrel;

        [FoldoutGroup("Cannon Parts"), SerializeField, Required]
        private Transform _wheels;

        [FoldoutGroup("Cannon Parts"), SerializeField, Required]
        private Transform _projectileSpawnPoint;

        [FoldoutGroup("Shooting"), SerializeField]
        private Vector2 _cooldownRange = new(10, 20);

        [FoldoutGroup("Shooting"), SerializeField]
        private Vector2 _aimCooldownRange = new(0.5f, 1.5f);

        [FoldoutGroup("Shooting"), SerializeField]
        private Vector2 _shootingForceRange = new(20, 50);

        [FoldoutGroup("Shooting"), SerializeField
         ]
        private Vector2 _shootingTorqueRange = new(-50, 50);

        [FoldoutGroup("Shooting"), SerializeField,
        ValidateInput(nameof(_projectilesValidation), "Cannon must have at least 1 Projectile")]
        private List<Rigidbody> _projectiles;

        private bool _projectilesValidation() => _projectiles.Count >= 1;

        [FoldoutGroup("Animation/Aim"), SerializeField]
        private Vector2 _aimAnimTimeRange = new(0.75f, 1.25f);

        [FoldoutGroup("Animation/Aim"), SerializeField]
        private AnimationCurve _aimCurve;

        [FoldoutGroup("Animation/Aim"), SerializeField]
        [Tooltip("X - Min X\nY - Max X\nZ - Min Z\nW - Max Z")]
        private Vector4 _aimPosRange = new(-1.5f, 1.5f, 1, 3);

        [FoldoutGroup("Animation/Aim"), SerializeField, UnityEngine.Range(0, 1)]
        private float _aimRotIntensity = 0.25f;

        [FoldoutGroup("Animation/Aim"), SerializeField]
        private Vector2 _aimBarrelRotationRange = new(-20, 0);

        [FoldoutGroup("Animation/Aim"), SerializeField]
        private float _wheelRotIntensity = 40;

        [FoldoutGroup("Animation/Shoot"), SerializeField]
        private Vector2 _shootAnimTimeRange = new(0.75f, 1);

        [FoldoutGroup("Animation/Shoot"), SerializeField]
        private AnimationCurve _shootCurve;

        private Quaternion _baseBarrelRot;

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_cooldownRange.x, _cooldownRange.y));

                var (startCannonPos, targetCannonPos,
                        startCannonRot, targetCannonRot,
                        startBarrelRot, targetBarrelRot,
                        startWheelRot, targetWheelRot)
                    = CalculateAnimationData();

                yield return AimAnimationRoutine(
                    startCannonPos, targetCannonPos,
                    startCannonRot, targetCannonRot,
                    startBarrelRot, targetBarrelRot,
                    startWheelRot, targetWheelRot);

                ShootProjectile();

                yield return ShootAnimationRoutine(
                    startCannonPos, targetCannonPos,
                    startCannonRot, targetCannonRot,
                    startBarrelRot, targetBarrelRot,
                    startWheelRot, targetWheelRot);
            }
        }

        private (
            Vector3 startCannonPos, Vector3 targetCannonPos,
            Quaternion startCannonRot, Quaternion targetCannonRot,
            Vector3 startBarrelRot, Vector3 targetBarrelRot,
            Quaternion startWheelRot, Quaternion targetWheelRot)
            CalculateAnimationData()
        {
            var startCannonPos = _cannon.localPosition;
            var targetCannonPos = startCannonPos;
            var startCannonRot = _cannon.localRotation;

            targetCannonPos.x += Random.Range(_aimPosRange.x, _aimPosRange.y);
            targetCannonPos.z += Random.Range(_aimPosRange.z, _aimPosRange.w);
            var cannonMoveDir = (targetCannonPos - startCannonPos).normalized;
            var moveDistance = Vector3.Distance(targetCannonPos, startCannonPos);

            var targetAngle = Mathf.Atan2(cannonMoveDir.x, cannonMoveDir.z) * Mathf.Rad2Deg;
            var rotationAmount = targetAngle * _aimRotIntensity * moveDistance;
            var targetCannonRot =
                Quaternion.Euler(0, startCannonRot.eulerAngles.y + rotationAmount, 0);

            var startBarrelRot = _barrel.localEulerAngles;
            var targetBarrelRot = startBarrelRot;
            targetBarrelRot.x = Random.Range(_aimBarrelRotationRange.x, _aimBarrelRotationRange.y);

            var startWheelRot = _wheels.localRotation;
            var targetWheelRot = startWheelRot * Quaternion.Euler(
                Vector3.right * (moveDistance * _wheelRotIntensity));

            return (
                startCannonPos, targetCannonPos,
                startCannonRot, targetCannonRot,
                startBarrelRot, targetBarrelRot,
                startWheelRot, targetWheelRot);
        }

        private IEnumerator AimAnimationRoutine(
            Vector3 startCannonPos, Vector3 targetCannonPos,
            Quaternion startCannonRot, Quaternion targetCannonRot,
            Vector3 startBarrelRot, Vector3 targetBarrelRot,
            Quaternion startWheelRot, Quaternion targetWheelRot)
        {
            var aimTime = Random.Range(_aimAnimTimeRange.x, _aimAnimTimeRange.y);
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = _aimCurve.Evaluate(Misc.Tween(ref lerpPos, aimTime));
                _cannon.localPosition =
                    Vector3.SlerpUnclamped(startCannonPos, targetCannonPos, t);
                _cannon.localRotation =
                    Quaternion.SlerpUnclamped(startCannonRot, targetCannonRot, t);
                _barrel.localRotation = Quaternion.Euler(
                    Vector3.LerpUnclamped(startBarrelRot, targetBarrelRot, t));
                _wheels.localRotation =
                    Quaternion.SlerpUnclamped(startWheelRot, targetWheelRot, t);
                yield return null;
            }

            yield return new WaitForSeconds(
                Random.Range(_aimCooldownRange.x, _aimCooldownRange.y));
        }

        private void ShootProjectile()
        {
            var p = _projectiles[Random.Range(0, _projectiles.Count)];
            var projectile = Instantiate(p, _projectileSpawnPoint.position,
                _projectileSpawnPoint.rotation);

            var shootingForce = Random.Range(_shootingForceRange.x, _shootingForceRange.y);
            projectile.AddRelativeForce(Vector3.forward * shootingForce, ForceMode.Impulse);

            var shootingTorque = Random.Range(_shootingTorqueRange.x, _shootingTorqueRange.y);
            projectile.AddTorque(
                new Vector3(Misc.Random1, Misc.Random1, Misc.Random1) * shootingTorque,
                ForceMode.Impulse);
        }

        private IEnumerator ShootAnimationRoutine(
            Vector3 startCannonPos, Vector3 targetCannonPos,
            Quaternion startCannonRot, Quaternion targetCannonRot,
            Vector3 startBarrelRot, Vector3 targetBarrelRot,
            Quaternion startWheelRot, Quaternion targetWheelRot)
        {
            var shootTime = Random.Range(_shootAnimTimeRange.x, _shootAnimTimeRange.y);
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = _shootCurve.Evaluate(Misc.Tween(ref lerpPos, shootTime));
                _cannon.localPosition =
                    Vector3.SlerpUnclamped(startCannonPos, targetCannonPos, t);
                _cannon.localRotation =
                    Quaternion.SlerpUnclamped(startCannonRot, targetCannonRot, t);
                _barrel.localRotation = Quaternion.Euler(
                    Vector3.LerpUnclamped(startBarrelRot, targetBarrelRot, t));
                _wheels.localRotation =
                    Quaternion.SlerpUnclamped(startWheelRot, targetWheelRot, t);
                yield return null;
            }
        }
    }
}