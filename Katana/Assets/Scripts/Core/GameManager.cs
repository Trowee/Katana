using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get => _instance;
            private set
            {
                if (_instance != null && _instance != value)
                {
                    Destroy(value.gameObject);
                    return;
                }
                
                _instance = value;
                DontDestroyOnLoad(_instance.gameObject);
            }
        }

        private void Reset()
        {
            
        }
        
        private void Awake()
        {
            if (_instance == null) Instance = this;
            else if (_instance != this) Destroy(gameObject);
        }

        private void Start()
        {
            
        }
    }
}