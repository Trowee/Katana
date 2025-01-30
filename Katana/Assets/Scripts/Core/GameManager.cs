using Assets.Scripts.Items;
using Assets.Scripts.TimeScale;
using NnUtils.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Core
{
    [RequireComponent(typeof(SettingsManager))]
    [RequireComponent(typeof(TimeScaleManager))]
    [RequireComponent(typeof(ItemManager))]
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

        [ReadOnly] [SerializeField] private SettingsManager _settingsManager;
        public static SettingsManager SettingsManager => Instance._settingsManager;
        
        [ReadOnly] [SerializeField] private TimeScaleManager _timeScaleManager;
        public static TimeScaleManager TimeScaleManager => Instance._timeScaleManager;

        [ReadOnly] [SerializeField] private ItemManager _itemManager;
        public static ItemManager ItemManager => Instance._itemManager;
        
        private void Reset()
        {
            _settingsManager = GetComponent<SettingsManager>();
            _timeScaleManager = this.GetOrAddComponent<TimeScaleManager>();
            _itemManager = this.GetOrAddComponent<ItemManager>();
        }

        private void Awake()
        {
            if (_instance == null) Instance = this;
            else if (_instance != this) Destroy(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            TimeScaleManager.UpdateTimeScale(1, -100);
        }
    }
}