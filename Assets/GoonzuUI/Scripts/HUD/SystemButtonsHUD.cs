using UnityEngine;
using UnityEngine.UI;
using GoonzuUI.Core;

namespace GoonzuUI.HUD
{
    public sealed class SystemButtonsHUD : MonoBehaviour
    {
        [System.Serializable]
        public struct ButtonBinding
        {
            public string windowId;
            public Button button;
        }

        [SerializeField] private ButtonBinding[] bindings;

        private UIEventBus _bus;

        public void Initialize(UIEventBus bus)
        {
            _bus = bus;
            foreach (var b in bindings)
            {
                if (!b.button) continue;
                var id = b.windowId;
                b.button.onClick.AddListener(() => _bus.Publish(new ToggleWindowEvent(id)));
            }
        }
    }
}
