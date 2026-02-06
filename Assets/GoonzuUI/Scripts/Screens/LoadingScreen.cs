using UnityEngine;
using UnityEngine.UI;

namespace GoonzuUI.Screens
{
    public sealed class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Slider progressBar;
        [SerializeField] private Text loadingText;

        public void SetProgress(float value)
        {
            if (progressBar) progressBar.value = value;
        }

        public void SetLoadingText(string text)
        {
            if (loadingText) loadingText.text = text;
        }
    }
}
