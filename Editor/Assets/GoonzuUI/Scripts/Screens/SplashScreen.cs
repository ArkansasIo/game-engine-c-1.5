using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoonzuUI.Screens
{
    public sealed class SplashScreen : MonoBehaviour
    {
        [SerializeField] private float displayTime = 2.5f;
        [SerializeField] private string nextSceneName = "Title";

        private void Start()
        {
            Debug.Log($"Splash screen shown for {displayTime} seconds.");
            Invoke(nameof(LoadNext), displayTime);
        }

        private void LoadNext()
        {
            Debug.Log($"Loading next scene: {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
