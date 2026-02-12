using UnityEngine;
namespace GoonzuUI.HUD {
    public class MinimapHUD : MonoBehaviour {
        public void ShowMinimap() {
            gameObject.SetActive(true);
            Debug.Log("Minimap shown.");
        }
        public void HideMinimap() {
            gameObject.SetActive(false);
            Debug.Log("Minimap hidden.");
        }
        public void UpdateMinimap() {
            Debug.Log("Minimap updated.");
        }
    }
}
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
