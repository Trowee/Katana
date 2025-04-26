using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.VFX;

namespace Assets.Scripts.Fruits
{
    public class ExplosionManagerScript : MonoBehaviour
    {
        [Tooltip("Prefab of the explosion effect")]
        [SerializeField, Required] private VisualEffect _explosionPrefab;
        
        [Tooltip("Maximum number of explosions at once")]
        [SerializeField] private int _explosionsCount = 8;

        private Transform _explosionsParent;
        private VisualEffect[] _explosions;

        private void Awake()
        {
            if (!_explosionPrefab)
            {
                Debug.LogError(
                    "Explosion Manager requires the Explosion Prefab in order to function");
                enabled = false;
                return;
            }
            
            _explosions = new VisualEffect[_explosionsCount];
            InitializeExplosions();
        }

        /// Creates the explosions parent and instantiates the explosion prefabs
        private void InitializeExplosions()
        {
            _explosionsParent = new GameObject().transform;
            _explosionsParent.SetParent(transform);
            
            for (int i = 0; i < _explosionsCount; i++)
                _explosions[i] = Instantiate(_explosionPrefab, _explosionsParent);
        }

        /// Plays the first available explosion at a given position
        public void PlayExplosion(Vector3 position)
        {
            foreach (var explosion in _explosions)
            {
                if (explosion.HasAnySystemAwake()) continue;
                
                explosion.transform.position = position;
                explosion.Play();
                
                return;
            }
        }
    }
}