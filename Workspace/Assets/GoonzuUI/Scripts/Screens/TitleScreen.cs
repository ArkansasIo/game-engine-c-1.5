using UnityEngine;
using UnityEngine.UI;

namespace GoonzuUI.Screens
{
    public sealed class TitleScreen : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject creditsPanel;

        private void Awake()
        {
            ShowMainMenu();
            if (startButton) startButton.onClick.AddListener(OnStartClicked);
            if (optionsButton) optionsButton.onClick.AddListener(ShowOptions);
            if (creditsButton) creditsButton.onClick.AddListener(ShowCredits);
            if (quitButton) quitButton.onClick.AddListener(OnQuitClicked);
        }

        public void ShowMainMenu()
        {
            if (mainMenuPanel) mainMenuPanel.SetActive(true);
            if (optionsPanel) optionsPanel.SetActive(false);
            if (creditsPanel) creditsPanel.SetActive(false);
            Debug.Log("Main menu shown.");
        }

        public void ShowOptions()
        {
            if (mainMenuPanel) mainMenuPanel.SetActive(false);
            if (optionsPanel) optionsPanel.SetActive(true);
            if (creditsPanel) creditsPanel.SetActive(false);
            Debug.Log("Options menu shown.");
        }

        public void ShowCredits()
        {
            if (mainMenuPanel) mainMenuPanel.SetActive(false);
            if (optionsPanel) optionsPanel.SetActive(false);
            if (creditsPanel) creditsPanel.SetActive(true);
            Debug.Log("Credits menu shown.");
        }

        private void OnStartClicked()
        {
            Debug.Log("Start button clicked. Loading game...");
            // Example: UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }

        private void OnQuitClicked()
        {
            Debug.Log("Quit button clicked. Exiting game.");
            Application.Quit();
        }
    }
}
