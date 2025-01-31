using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Core
{
    public class DeathScreen : MonoBehaviour
    {
        public void Retry() => SceneManager.LoadScene(1);
        public void Menu() => SceneManager.LoadScene(0);
    }
}