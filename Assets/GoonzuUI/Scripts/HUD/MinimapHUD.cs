using UnityEngine;

namespace GoonzuUI.HUD
{
    public sealed class MinimapHUD : MonoBehaviour
    {
        [SerializeField] private RectTransform minimapPanel;
        [SerializeField] private UnityEngine.UI.Image minimapImage;
        // Add more fields as needed for markers, player icon, etc.

        public void SetMinimapSprite(Sprite sprite)
        {
            if (minimapImage) minimapImage.sprite = sprite;
        }
    }
}
