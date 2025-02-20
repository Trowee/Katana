using UnityEngine;
using UnityEngine.VFX;

namespace Assets.Scripts.Fruits
{
    public class ExplosionManagerScript : MonoBehaviour
    {
        [Tooltip("Prefab of the explosion effect")]
        [SerializeField] private VisualEffect _explosionPrefab;
        
        [Tooltip("Maximum number of explosions at once")]
        [SerializeField] private int _explosionsCount = 4;

        /// Parent for explosions
        private Transform _explosionsParent;
        
        /// Holds all explosions
        private VisualEffect[] _explosions;

        private void Awake()
        {
            // Return if prefab is not set
            if (_explosionPrefab == null)
            {
                Debug.LogError("Explosion Manager requires the Explosion Prefab in order to function");
                enabled = false;
                return;
            }
            
            // Create an array with set capacity
            _explosions = new VisualEffect[_explosionsCount];
            
            // Initialize explosions
            InitializeExplosions();
        }

        /// Creates the explosions parent and instantiates the explosion prefabs
        private void InitializeExplosions()
        {
            // Create the parent
            _explosionsParent = new GameObject().transform;
            _explosionsParent.SetParent(transform);
            
            // Instantiate explosions
            for (int i = 0; i < _explosionsCount; i++)
                _explosions[i] = Instantiate(_explosionPrefab, _explosionsParent);
        }

        /// Plays the first available explosion at a given position
        public void PlayExplosion(Vector3 position)
        {
            foreach (var explosion in _explosions)
            {
                // Continue if the explosion is still playing
                if (explosion.HasAnySystemAwake()) continue;
                
                // Set position and play
                explosion.transform.position = position;
                explosion.Play();
                
                return;
            }
        }
    }
}