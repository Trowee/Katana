using UnityEngine;
using NnUtils.Scripts;
using PlayerCamera;

namespace Core
{
    [RequireComponent(typeof(CameraManager))]
    public class PlaySceneManager : MonoBehaviour
    {
        private static PlaySceneManager _instance;
        public static PlaySceneManager Instance => _instance = FindFirstObjectByType<PlaySceneManager>();

        [SerializeField] private GameObject _player;
        public static GameObject Player => Instance._player ??= GameObject.FindWithTag("Player");
        
        [SerializeField] private CameraManager _cameraManager;
        public static CameraManager CameraManager => Instance._cameraManager ??= Instance.gameObject.GetOrAddComponent<CameraManager>();

        private void Reset()
        {
            _cameraManager = GetComponent<CameraManager>();
            _player        = GameObject.FindWithTag("Player");
        }

        private void Awake()
        {
            if (_instance != null) Destroy(gameObject);
            _instance = this;
        }
    }
}