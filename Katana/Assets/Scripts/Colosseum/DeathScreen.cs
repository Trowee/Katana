using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Colosseum
{
    public class DeathScreen : MonoBehaviour
    {
        public void Retry() => SceneManager.LoadScene(1);
        public void Menu() => SceneManager.LoadScene(0);
    }
}
