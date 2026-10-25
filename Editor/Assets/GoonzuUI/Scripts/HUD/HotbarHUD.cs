using UnityEngine;
namespace GoonzuUI.HUD {
    public class HotbarHUD : MonoBehaviour {
        public void ShowHotbar() {
            gameObject.SetActive(true);
            Debug.Log("Hotbar shown.");
        }
        public void HideHotbar() {
            gameObject.SetActive(false);
            Debug.Log("Hotbar hidden.");
        }
        public void UpdateHotbar() {
            Debug.Log("Hotbar updated.");
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using GoonzuUI.Core;

namespace GoonzuUI.HUD
{
    public sealed class HotbarHUD : MonoBehaviour
    {
        [System.Serializable]
        public struct HotbarSlot
        {
            public Button button;
            public int slotIndex;
        }

        [SerializeField] private HotbarSlot[] slots;
        private UIEventBus _bus;

        public void Initialize(UIEventBus bus)
        {
            _bus = bus;
            foreach (var s in slots)
            {
                if (!s.button) continue;
                int idx = s.slotIndex;
                s.button.onClick.AddListener(() => _bus.Publish(new HotbarUseSlotEvent(idx)));
            }
        }
    }
}
