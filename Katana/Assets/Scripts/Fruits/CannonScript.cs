using System.Collections;
using System.Collections.Generic;
using NnUtils.Scripts;
using UnityEngine;

namespace Assets.Scripts.Fruits
{
    public class CannonScript : MonoBehaviour
    {
        [SerializeField] private Transform _cannon;
        [SerializeField] private Transform _barrel;
        [SerializeField] private Transform _wheels;
        [SerializeField] private Transform _projectileSpawnPoint;
        
        [Header("Shooting")]
        
        [SerializeField] private Vector2 _cooldownRange = new(10, 20);
        [SerializeField] private Vector2 _aimCooldownRange = new(0.5f, 1.5f);
        [SerializeField] private Vector2 _shootingForceRange = new(20, 50);
        [SerializeField] private Vector2 _shootingTorqueRange = new(-50, 50);
        [SerializeField] private List<Rigidbody> _projectiles;

        [Header("Aim Animation", order = 1)]
        
        [SerializeField] private Vector2 _aimAnimTimeRange = new(0.75f, 1.25f);
        [SerializeField] private AnimationCurve _aimCurve;
        [SerializeField] private Vector4 _aimPosRange = new(-1.5f, 1.5f, 1, 3);
        [SerializeField, Range(0, 1)] private float _aimRotIntensity = 0.25f;
        [SerializeField] private Vector2 _aimBarrelRotationRange = new(-20, 0);
        [SerializeField] private float _wheelRotIntensity = 40;
        
        [Header("Shoot Animation", order = 1)]
        
        [SerializeField] private Vector2 _shootAnimTimeRange = new(0.75f, 1);
        [SerializeField] private AnimationCurve _shootCurve;

        private Quaternion _baseBarrelRot;

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_cooldownRange.x, _cooldownRange.y));

                // Store Cannon Data
                var startCannonPos = _cannon.localPosition;
                var targetCannonPos = startCannonPos;
                var startCannonRot = _cannon.localRotation;

                // Calculate Target Cannon Pos, Dir and Distance Data
                targetCannonPos.x += Random.Range(_aimPosRange.x, _aimPosRange.y);
                targetCannonPos.z += Random.Range(_aimPosRange.z, _aimPosRange.w);
                var cannonMoveDir = (targetCannonPos - startCannonPos).normalized;
                var moveDistance = Vector3.Distance(targetCannonPos, startCannonPos);

                // Calculate Cannon Rot Data
                var targetAngle = Mathf.Atan2(cannonMoveDir.x, cannonMoveDir.z) * Mathf.Rad2Deg;
                var rotationAmount = targetAngle * _aimRotIntensity * moveDistance;
                var targetCannonRot =
                    Quaternion.Euler(0, startCannonRot.eulerAngles.y + rotationAmount, 0);
                
                // Store Barrel Rot Data
                var startBarrelRot = _barrel.localEulerAngles;
                var targetBarrelRot = startBarrelRot;
                targetBarrelRot.x =
                    Random.Range(_aimBarrelRotationRange.x, _aimBarrelRotationRange.y);
                
                // Calculate Wheels Rotation
                var startWheelRot = _wheels.localRotation;
                var targetWheelRot = Quaternion.Euler(
                    startWheelRot * (Vector3.right * moveDistance * _wheelRotIntensity));
                
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
                
                var f = _projectiles[Random.Range(0, _projectiles.Count)];
                var fruit = Instantiate(f, _projectileSpawnPoint.position, _projectileSpawnPoint.rotation);

                var shootingForce = Random.Range(_shootingForceRange.x, _shootingForceRange.y);
                fruit.AddRelativeForce(Vector3.forward * shootingForce, ForceMode.Impulse);
                
                var shootingTorque = Random.Range(_shootingTorqueRange.x, _shootingTorqueRange.y);
                fruit.AddTorque(
                    new Vector3(Misc.Random1, Misc.Random1, Misc.Random1) * shootingTorque,
                    ForceMode.Impulse);

                var shootTime = Random.Range(_shootAnimTimeRange.x, _shootAnimTimeRange.y);
                lerpPos = 0;
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
}