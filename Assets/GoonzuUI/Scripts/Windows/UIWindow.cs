using UnityEngine;

namespace GoonzuUI.Windows
{
    public abstract class UIWindow : MonoBehaviour
    {
        [Header("Window Identity")]
        [SerializeField] private string windowId;

        public string WindowId => windowId;

        public virtual void Show() => gameObject.SetActive(true);
        public virtual void Hide() => gameObject.SetActive(false);

        // Optional hooks
        public virtual void OnOpen() { }
        public virtual void OnClose() { }
    }
}
