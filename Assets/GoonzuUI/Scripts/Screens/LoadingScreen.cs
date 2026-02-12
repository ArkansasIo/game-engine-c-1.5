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
            Debug.Log($"Loading progress: {value}");
        }

        public void SetLoadingText(string text)
        {
            if (loadingText) loadingText.text = text;
            Debug.Log($"Loading text set: {text}");
        }
    }
}
