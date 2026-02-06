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
