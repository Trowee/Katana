using ArtificeToolkit.Attributes;
using Assets.Scripts.Fruits;
using Assets.Scripts.Player;
using Assets.Scripts.Player.Camera;
using UnityEngine;

namespace Assets.Scripts.Core
{
    [RequireComponent(typeof(CameraManager))]
    [RequireComponent(typeof(ExplosionManagerScript))]
    public class ColosseumSceneManager : MonoBehaviour
    {
        private static ColosseumSceneManager _instance;
        public static ColosseumSceneManager Instance =>
            _instance ? _instance : _instance = FindFirstObjectByType<ColosseumSceneManager>();

        [SerializeField, ReadOnly] private CameraManager _cameraManager;
        public static CameraManager CameraManager
        {
            get
            {
                if (!Instance) return null;
                return Instance._cameraManager ??= Instance.gameObject.GetComponent<CameraManager>();
            }
        }

        [SerializeField, ReadOnly] private ExplosionManagerScript _explosionManager;
        public static ExplosionManagerScript ExplosionManager
        {
            get
            {
                if (!Instance) return null;
                return Instance._explosionManager ??= Instance.gameObject.GetComponent<ExplosionManagerScript>();
            }
        }

        [FoldoutGroup("Dev")]
        [SerializeField, ReadOnly]
        private bool _isDead;
        public static bool IsDead => Instance._isDead;

        [SerializeField, Required] private PlayerScript _player;
        public static PlayerScript Player
        {
            get
            {
                if (!Instance) return null;
                return Instance._player ??=
                    GameObject.FindWithTag("Player")?.GetComponent<PlayerScript>();
            }
        }

        [SerializeField, Required] private GameObject _deathScreen;

        private void Reset()
        {
            _cameraManager = GetComponent<CameraManager>();
            _explosionManager = GetComponent<ExplosionManagerScript>();
            _player = GameObject.FindWithTag("Player")?.GetComponent<PlayerScript>();
        }

        private void Awake()
        {
            if (_instance && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void Die()
        {
            Instance._deathScreen.SetActive(true);
            Instance._isDead = true;
        }
    }
}
