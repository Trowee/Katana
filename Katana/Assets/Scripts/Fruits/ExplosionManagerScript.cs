using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

namespace Assets.Scripts.Fruits
{
    public class ExplosionManagerScript : MonoBehaviour
    {
        // Stores up to 4 inactive explosions
        private readonly List<VisualEffect> _newlyAvailableExplosions = new(4);
        private readonly HashSet<VisualEffect> _unavailableExplosions = new(4);
        private readonly Queue<VisualEffect> _availableExplosions = new(4);

        /// How often the available explosions check is ran
        [SerializeField] private float _checkInterval = 1;
 
        private void Awake()
        {
            foreach (var xp in GetComponentsInChildren<VisualEffect>()) _availableExplosions.Enqueue(xp);
        }

        private void Start() => StartCoroutine(AvailableCheckRoutine());

        private IEnumerator AvailableCheckRoutine()
        {
            var checkInterval = _checkInterval;
            
            while (true)
            {
                CheckActive();
                
                if (_newlyAvailableExplosions.Count > 0)
                {
                    foreach (var xp in _newlyAvailableExplosions)
                    {
                        _unavailableExplosions.Remove(xp);
                        _availableExplosions.Enqueue(xp);
                    }
                }

                yield return new WaitForSecondsRealtime(checkInterval);
            }
        }

        private void CheckActive()
        {
            _newlyAvailableExplosions.Clear();
            foreach (var xp in _unavailableExplosions.Where(xp => !xp.HasAnySystemAwake()))
                _newlyAvailableExplosions.Add(xp);
        }
        
        public void PlayExplosion(Vector3 position)
        {
            if (_availableExplosions.Count == 0) return;
            var xp = _availableExplosions.Dequeue();
            _unavailableExplosions.Add(xp);
            xp.Play();
        }
    }
}