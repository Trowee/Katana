using System.Collections;
using System.Collections.Generic;
using ArtificeToolkit.Attributes;
using NnUtils.Scripts;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Fruits
{
    public class CannonScript : MonoBehaviour
    {
        [FoldoutGroup("Cannon Parts"), SerializeField, Required, ChildGameObjectsOnly]
        private Transform _cannon;
        [FoldoutGroup("Cannon Parts"), SerializeField, Required, ChildGameObjectsOnly]
        private Transform _barrel;
        [FoldoutGroup("Cannon Parts"), SerializeField, Required, ChildGameObjectsOnly]
        private Transform _wheels;
        [FoldoutGroup("Cannon Parts"), SerializeField, Required, ChildGameObjectsOnly]
        private Transform _projectileSpawnPoint;

        [FoldoutGroup("Shooting"), SerializeField]
        private int _maxProjectilesPerCycle = 5;
        [FoldoutGroup("Shooting"), SerializeField]
        private Vector2 _shootingForceRange = new(400, 500);
        [FoldoutGroup("Shooting"), SerializeField]
        private Vector2 _shootingTorqueRange = new(-360, 360);
        [FoldoutGroup("Shooting"), SerializeField]
        private Vector2 _shootingPauseRange = new(1, 3);
        [FoldoutGroup("Shooting"), SerializeField,
            ValidateInput(nameof(_projectilesValidation),
                "Cannon must have at least 1 Projectile")]
        private List<Rigidbody> _projectiles;
        private bool _projectilesValidation() => _projectiles.Count >= 1;

        [FoldoutGroup("Shooting/Animation"), UnityEngine.Range(0, 1)]
        private float _cannonRotationIntensity = 0.25f;
        [FoldoutGroup("Shooting/Animation")]
        private float _wheelRotationIntensity = 40;
        [FoldoutGroup("Shooting/Animation"), SerializeField]
        private CannonAnimationKey AimAnimationKey;
        [FoldoutGroup("Shooting/Animation"), SerializeField]
        private Vector2 _aimPauseRange = new(0.1f, 0.25f);
        [FoldoutGroup("Shooting/Animation"), SerializeField]
        private CannonAnimationKey ShootAnimationKey;
        [FoldoutGroup("Shooting/Animation"), SerializeField]
        private Vector2 _pauseBetweenShots = new(0.05f, 0.1f);
        [FoldoutGroup("Shooting/Animation"), SerializeField]
        private CannonAnimationKey FinalShotAnimationKey;

        [FoldoutGroup("Shooting/VFX"), SerializeField]
        private VisualEffect _shotVFX;
        [FoldoutGroup("Shooting/VFX"), SerializeField]
        private int _shotVFXBurstCount = 5;
        //TODO: Add validation for delay between bursts and shots
        [FoldoutGroup("Shooting/VFX"), SerializeField]
        private float _shotVFXDelayBetweenBursts = 0.05f;
        [FoldoutGroup("Shooting/VFX"), SerializeField]
        private AnimationCurve _shotVFXSpeedCurve;

        private IEnumerator Start()
        {
            var startCannonPos = _cannon.localPosition;
            var startCannonRot = _cannon.localRotation.eulerAngles;
            var startBarrelPos = _barrel.localPosition;
            var startBarrelRot = _barrel.localRotation.eulerAngles;
            var startWheelsRot = _wheels.localRotation.eulerAngles;

            while (true)
            {
                // Wait before the first shot
                yield return new WaitForSeconds(
                    Random.Range(_shootingPauseRange.x, _shootingPauseRange.y));

                // Aim
                var (aimDuration, aimCurve,
                    aimStartCannonPos, aimEndCannonPos,
                    aimStartBarrelPos, aimEndBarrelPos,
                    aimStartBarrelRot, aimEndBarrelRot,
                    aimStartWheelsRot, aimEndWheelsRot)
                = CalculateAimAnimationData(AimAnimationKey,
                    startCannonPos, startBarrelPos, startBarrelRot, startWheelsRot);
                var aimStartCannonRot = _cannon.localRotation.eulerAngles;
                var aimEndCannonRot = GetCannonAimRotation(aimStartCannonPos, aimEndCannonPos);

                yield return AnimateRoutine(
                    aimDuration, aimCurve,
                    aimStartCannonPos, aimEndCannonPos,
                    aimStartCannonRot, aimEndCannonRot,
                    aimStartBarrelPos, aimEndBarrelPos,
                    aimStartBarrelRot, aimEndBarrelRot,
                    aimStartWheelsRot, aimEndWheelsRot);

                yield return new WaitForSeconds(
                    Random.Range(_aimPauseRange.x, _aimPauseRange.y));

                // Perform all shots except for the final
                var shootStartCannonRot = aimEndCannonRot;
                var shootEndCannonRot = aimEndCannonRot;
                var shots = Random.Range(1, _maxProjectilesPerCycle + 1);

                for (int i = 0; i < shots - 1; i++)
                {
                    var (shootDuration, shootCurve,
                        shootStartCannonPos, shootEndCannonPos,
                        shootStartBarrelPos, shootEndBarrelPos,
                        shootStartBarrelRot, shootEndBarrelRot,
                        shootStartWheelsRot, shootEndWheelsRot)
                    = CalculateAimAnimationData(ShootAnimationKey,
                        aimEndCannonPos, aimEndBarrelPos, aimEndBarrelRot, aimEndWheelsRot);

                    ShootProjectile();

                    yield return AnimateRoutine(
                        shootDuration, shootCurve,
                        shootStartCannonPos, shootEndCannonPos,
                        shootStartCannonRot, shootEndCannonRot,
                        shootStartBarrelPos, shootEndBarrelPos,
                        shootStartBarrelRot, shootEndBarrelRot,
                        shootStartWheelsRot, shootEndWheelsRot);

                    yield return new WaitForSeconds(
                        Random.Range(_pauseBetweenShots.x, _pauseBetweenShots.y));
                }

                // Final shot
                var (finalShotDuration, finalShotCurve,
                    finalShotStartCannonPos, finalShotEndCannonPos,
                    finalShotStartBarrelPos, finalShotEndBarrelPos,
                    finalShotStartBarrelRot, finalShotEndBarrelRot,
                    finalShotStartWheelsRot, finalShotEndWheelsRot)
                = CalculateAimAnimationData(FinalShotAnimationKey,
                    aimEndCannonPos, aimEndBarrelPos, aimEndBarrelRot, aimEndWheelsRot);

                var finalShotStartCannonRot = aimEndCannonRot;
                var finalShotEndCannonRot = aimEndCannonRot;

                ShootProjectile();

                yield return AnimateRoutine(
                    finalShotDuration, finalShotCurve,
                    finalShotStartCannonPos, startCannonPos,
                    finalShotStartCannonRot, startCannonRot,
                    finalShotStartBarrelPos, startBarrelPos,
                    finalShotStartBarrelRot, startBarrelRot,
                    finalShotStartWheelsRot, startWheelsRot);
            }
        }

        private (
            float duration, AnimationCurve curve,
            Vector3 startCannonPos, Vector3 endCannonPos,
            Vector3 startBarrelPos, Vector3 endBarrelPos,
            Vector3 startBarrelRot, Vector3 endBarrelRot,
            Vector3 startWheelRot, Vector3 endWheelRot)
        CalculateAimAnimationData(
            CannonAnimationKey animKey,
            Vector3 startCannonPos,
            Vector3 startBarrelPos,
            Vector3 startBarrelRot,
            Vector3 startWheelsRot)
        {
            var duration = Random.Range(animKey.DurationRange.x, animKey.DurationRange.y);

            var cannonOffset = Misc.RandomVector3(
                animKey.CannonPositionOffsetMin, animKey.CannonPositionOffsetMax);
            var endCannonPos = startCannonPos + (animKey.TranslateCannonPosition
                ? cannonOffset : _cannon.TransformDirection(cannonOffset));
            var moveDistance = Vector3.Distance(startCannonPos, endCannonPos);

            var barrelOffset = Misc.RandomVector3(
                animKey.BarrelPositionOffsetMin, animKey.BarrelPositionOffsetMax);
            var endBarrelPos = startBarrelPos + (animKey.TranslateBarrelPosition
                ? barrelOffset : _barrel.TransformDirection(barrelOffset));

            var endBarrelRot = startBarrelRot + Misc.RandomVector3(
                animKey.BarrelRotationOffsetMin, animKey.BarrelRotationOffsetMax);

            var endWheelsRot =
                startWheelsRot + (Vector3.right * (moveDistance * _wheelRotationIntensity));

            return (
                duration, animKey.Curve,
                startCannonPos, endCannonPos,
                startBarrelPos, endBarrelPos,
                startBarrelRot, endBarrelRot,
                startWheelsRot, endWheelsRot);
        }

        private Vector3 GetCannonAimRotation(Vector3 startPos, Vector3 endPos)
        {
            var cannonMoveDir = (endPos - startPos).normalized;
            var moveDistance = Vector3.Distance(endPos, startPos);
            var startCannonRot = _cannon.localRotation.eulerAngles;
            var targetAngle = Mathf.Atan2(cannonMoveDir.x, cannonMoveDir.z) * Mathf.Rad2Deg;
            var rotationAmount = targetAngle * _cannonRotationIntensity * moveDistance;
            return new Vector3(0, startCannonRot.y + rotationAmount, 0);
        }

        private IEnumerator AnimateRoutine(
            float duration, AnimationCurve curve,
            Vector3 startCannonPos, Vector3 endCannonPos,
            Vector3 startCannonRot, Vector3 endCannonRot,
            Vector3 startBarrelPos, Vector3 endBarrelPos,
            Vector3 startBarrelRot, Vector3 endBarrelRot,
            Vector3 startWheelRot, Vector3 endWheelRot)
        {
            float lerpPos = 0;
            while (lerpPos < 1)
            {
                var t = curve.Evaluate(Misc.Tween(ref lerpPos, duration));
                _cannon.localPosition =
                    Vector3.LerpUnclamped(startCannonPos, endCannonPos, t);
                _cannon.localRotation = Quaternion.Euler(
                    Vector3.LerpUnclamped(startCannonRot, endCannonRot, t));
                _barrel.localPosition =
                    Vector3.LerpUnclamped(startBarrelPos, endBarrelPos, t);
                _barrel.localRotation = Quaternion.Euler(
                    Vector3.LerpUnclamped(startBarrelRot, endBarrelRot, t));
                _wheels.localRotation = Quaternion.Euler(
                    Vector3.LerpUnclamped(startWheelRot, endWheelRot, t));
                yield return null;
            }
        }

        private void ShootProjectile()
        {
            var shotStrength = Random.Range(0f, 1f);
            var p = _projectiles[Random.Range(0, _projectiles.Count)];
            var projectile = Instantiate(p, _projectileSpawnPoint.position,
                _projectileSpawnPoint.rotation);

            var shootingForce = Mathf.Lerp(
                _shootingForceRange.x, _shootingForceRange.y, shotStrength);
            projectile.AddRelativeForce(Vector3.forward * shootingForce, ForceMode.Impulse);

            var shootingTorque = Mathf.Lerp(
                _shootingTorqueRange.x, _shootingTorqueRange.y, shotStrength);
            projectile.AddTorque(
                new Vector3(Misc.Random1, Misc.Random1, Misc.Random1) * shootingTorque,
                ForceMode.Impulse);

            StartCoroutine(ShootVFXRoutine(shotStrength));
        }

        private IEnumerator ShootVFXRoutine(float shotStrength)
        {
            _shotVFX.transform.position = _projectileSpawnPoint.position;
            _shotVFX.transform.rotation = _projectileSpawnPoint.rotation;

            var _speedRange = _shotVFX.GetVector2("SpeedRange");
            var currentSpeedRange = _speedRange;

            for (int i = 0; i < _shotVFXBurstCount; i++)
            {
                var t = _shotVFXBurstCount > 1 ? i / (_shotVFXBurstCount - 1f) : 0;
                currentSpeedRange.y = Mathf.Lerp(
                    _speedRange.x, _speedRange.y, _shotVFXSpeedCurve.Evaluate(t) * shotStrength);
                _shotVFX.SetVector2("SpeedRange", currentSpeedRange);
                _shotVFX.Play();
                yield return new WaitForSeconds(_shotVFXDelayBetweenBursts);
            }

            _shotVFX.SetVector2("SpeedRange", _speedRange);
        }
    }
}
