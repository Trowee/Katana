using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        private static MainMenuManager _instance;
        public static MainMenuManager Instance => _instance = FindFirstObjectByType<MainMenuManager>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
        }

        public void Play() => SceneManager.LoadScene(1);

        public void Settings()
        {
            
        }
        
        public void Quit() => Application.Quit();
    }
}