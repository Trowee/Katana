using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        private static MainMenuManager _instance;
        public static MainMenuManager Instance => _instance ??= FindFirstObjectByType<MainMenuManager>();
        
        
        
        public void Play() => SceneManager.LoadScene(1);

        public void Settings()
        {
            
        }
        
        public void Quit() => Application.Quit();
    }
}