using ArtificeToolkit.Attributes;
using Assets.Scripts.Audio;
using Assets.Scripts.Items;
using Assets.Scripts.TimeScale;
using NnUtils.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Core
{
    [RequireComponent(typeof(SettingsManager))]
    [RequireComponent(typeof(TimeScaleManager))]
    [RequireComponent(typeof(AudioManager.AudioManager))]
    [RequireComponent(typeof(ItemManager))]
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get => _instance;
            private set
            {
                if (_instance && _instance != value)
                {
                    Destroy(value.gameObject);
                    return;
                }

                _instance = value;
                _instance.transform.SetParent(null);
                DontDestroyOnLoad(_instance.gameObject);
            }
        }

        [SerializeField, Required] private SettingsManager _settingsManager;
        public static SettingsManager SettingsManager => Instance._settingsManager;

        [SerializeField, Required] private TimeScaleManager _timeScaleManager;
        public static TimeScaleManager TimeScaleManager => Instance._timeScaleManager;
        
        [SerializeField, Required] private AudioManager.AudioManager _audioManager;
        public static AudioManager.AudioManager AudioManager => Instance._audioManager;

        [SerializeField, Required] private ItemManager _itemManager;
        public static ItemManager ItemManager => Instance._itemManager;

        private void Reset()
        {
            _settingsManager = GetComponent<SettingsManager>();
            _timeScaleManager = gameObject.GetOrAddComponent<TimeScaleManager>();
            _audioManager = gameObject.GetOrAddComponent<AudioManager.AudioManager>();
            _itemManager = gameObject.GetOrAddComponent<ItemManager>();
        }

        private void Awake()
        {
            if (!_instance) Instance = this;
            else if (_instance != this) Destroy(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            TimeScaleManager.UpdateTimeScale(1, -100);
        }
    }
}