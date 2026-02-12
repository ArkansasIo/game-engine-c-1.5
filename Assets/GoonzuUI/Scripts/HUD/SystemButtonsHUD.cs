using UnityEngine;
namespace GoonzuUI.HUD {
    public class SystemButtonsHUD : MonoBehaviour {
        public void ShowButtons() {
            gameObject.SetActive(true);
            Debug.Log("System buttons shown.");
        }
        public void HideButtons() {
            gameObject.SetActive(false);
            Debug.Log("System buttons hidden.");
        }
        public void UpdateButtons() {
            Debug.Log("System buttons updated.");
        }
    }
}
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
