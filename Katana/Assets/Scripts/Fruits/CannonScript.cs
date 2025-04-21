using System.Collections;
using System.Collections.Generic;
using NnUtils.Scripts;
using UnityEngine;

namespace Assets.Scripts.Fruits
{
    public class CannonScript : MonoBehaviour
    {
        [SerializeField] private Transform _barrel;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private List<Rigidbody> _fruits;

        [SerializeField] private Vector2 _shootingRotRange = new(-20, 10);
        [SerializeField] private Vector2 _cooldownRange = new(10, 20);
        [SerializeField] private Vector2 _shootingForceRange = new(25, 75);
        [SerializeField] private Vector2 _torqueRange = new(-50, 50);

        [SerializeField] private float _aimTime = 1;
        [SerializeField] private AnimationCurve _aimCurve;

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_cooldownRange.x, _cooldownRange.y));

                var startRot = _barrel.localEulerAngles;
                var targetRot = startRot;
                targetRot.x = Random.Range(_shootingRotRange.x, _shootingRotRange.y);
                
                float lerpPos = 0;
                while (lerpPos < 1)
                {
                    var t = _aimCurve.Evaluate(Misc.Tween(ref lerpPos, _aimTime));
                    _barrel.localRotation = Quaternion.Euler(Vector3.LerpUnclamped(startRot, targetRot, t));
                    yield return null;
                }

                yield return new WaitForSeconds(1);
                
                var f = _fruits[Random.Range(0, _fruits.Count)];
                var fruit = Instantiate(f, _spawnPoint.position, _spawnPoint.rotation);

                var shootingForce = Random.Range(_shootingForceRange.x, _shootingForceRange.y);
                fruit.AddRelativeForce(Vector3.forward * shootingForce, ForceMode.Impulse);
                
                var shootingTorque = Random.Range(_torqueRange.x, _torqueRange.y);
                fruit.AddTorque(
                    new Vector3(Misc.Random1, Misc.Random1, Misc.Random1) * shootingTorque,
                    ForceMode.Impulse);
                
                lerpPos = 0;
                while (lerpPos < 1)
                {
                    var t = _aimCurve.Evaluate(Misc.Tween(ref lerpPos, _aimTime));
                    _barrel.localRotation = Quaternion.Euler(Vector3.LerpUnclamped(targetRot, startRot, t));
                    yield return null;
                }
            }
        }
    }
}