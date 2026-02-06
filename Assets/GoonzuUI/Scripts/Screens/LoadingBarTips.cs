using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GoonzuUI.Screens
{
    public sealed class LoadingBarTips : MonoBehaviour
    {
        [SerializeField] private Slider loadingBar;
        [SerializeField] private TMP_Text tipText;
        [SerializeField] private string[] tips;

        private void Start()
        {
            ShowRandomTip();
        }

        public void SetProgress(float value)
        {
            if (loadingBar) loadingBar.value = value;
        }

        public void ShowRandomTip()
        {
            if (tips != null && tips.Length > 0 && tipText)
            {
                int idx = Random.Range(0, tips.Length);
                tipText.text = tips[idx];
            }
        }
    }
}
