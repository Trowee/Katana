using Assets.Scripts.Fruits;
using Assets.Scripts.Player;
using Assets.Scripts.Player.Camera;
using UnityEngine;

namespace Assets.Scripts.Core
{
    [RequireComponent(typeof(CameraManager))]
    [RequireComponent(typeof(ExplosionManagerScript))]
    public class PlaySceneManager : MonoBehaviour
    {
        private static PlaySceneManager _instance;
        public static PlaySceneManager Instance => _instance = FindFirstObjectByType<PlaySceneManager>();

        [SerializeField] private PlayerScript _player;
        public static PlayerScript Player
        {
            get
            {
                if (!Instance) return null;
                return Instance._player ??=
                    GameObject.FindWithTag("Player")?.GetComponent<PlayerScript>();
            }
        }

        [SerializeField] private CameraManager _cameraManager;

        public static CameraManager CameraManager
        {
            get
            {
                if (!Instance) return null;
                return Instance._cameraManager ??= Instance.gameObject.GetComponent<CameraManager>();
            }
        }

        [SerializeField] private ExplosionManagerScript _explosionManager;

        public static ExplosionManagerScript ExplosionManager
        {
            get
            {
                if (!Instance) return null;
                return Instance._explosionManager ??= Instance.gameObject.GetComponent<ExplosionManagerScript>();
            }
        }

        public bool IsDead;

        [SerializeField] private GameObject _deathScreen;

        private void Reset()
        {
            _cameraManager    = GetComponent<CameraManager>();
            _explosionManager = GetComponent<ExplosionManagerScript>();
            _player           = GameObject.FindWithTag("Player")?.GetComponent<PlayerScript>();
        }

        private void Awake()
        {
            if (_instance != null) Destroy(gameObject);
            _instance = this;
        }

        private void Start()
        {
            Cursor.visible   = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Die()
        {
            _deathScreen.SetActive(true);
            IsDead = true;
        }
    }
}