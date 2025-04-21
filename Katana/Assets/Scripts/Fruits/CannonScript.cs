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

        [SerializeField] private float _idleRot = -50;
        [SerializeField] private Vector2 _shootingRotRange = new(-20, 20);
        [SerializeField] private Vector2 _cooldownRange = new(10, 20);
        [SerializeField] private Vector2 _shootingForce = new(50, 100);
        [SerializeField] private Vector2 _torqueRange = new(-50, 50);

        [SerializeField] private float _aimTime = 1;
        [SerializeField] private AnimationCurve _aimCurve;

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_cooldownRange.x, _cooldownRange.y));

                var startRot = _barrel.localEulerAngles;
                startRot.z = _idleRot;
                var targetRot = startRot;
                targetRot.z = Random.Range(_shootingRotRange.x, _shootingRotRange.y);
                
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
                fruit.AddRelativeForce(Vector3.right * _shootingForce, ForceMode.Impulse);
                fruit.AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * _torqueRange, ForceMode.Impulse);
                
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