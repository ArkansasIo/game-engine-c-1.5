// (Commented out non-Unity version to avoid conflicts)
// using System;
// namespace GoonzuGame.GUI
// {
//     public class UIWindow
//     {
//         public bool IsVisible { get; set; }
//         public virtual void Show()
//         {
//             IsVisible = true;
//             Console.WriteLine($"Showing {this.GetType().Name}");
//         }
//         public virtual void Hide()
//         {
//             IsVisible = false;
//             Console.WriteLine($"Hiding {this.GetType().Name}");
//         }
//     }
// }
using UnityEngine;

namespace GoonzuUI.Windows
{
    public abstract class UIWindow : MonoBehaviour
    {
        [Header("Window Identity")]
        [SerializeField] private string windowId;

        public string WindowId => windowId;

        public virtual void Show() {
            gameObject.SetActive(true);
            Debug.Log($"{GetType().Name} shown.");
            OnOpen();
        }
        public virtual void Hide() {
            gameObject.SetActive(false);
            Debug.Log($"{GetType().Name} hidden.");
            OnClose();
        }
        public virtual void UpdateWindow() {
            Debug.Log($"{GetType().Name} updated.");
        }
        // Optional hooks
        public virtual void OnOpen() { }
        public virtual void OnClose() { }
    }
}
