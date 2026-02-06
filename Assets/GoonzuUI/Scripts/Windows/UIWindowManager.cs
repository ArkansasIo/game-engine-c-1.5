using System.Collections.Generic;
using UnityEngine;
using GoonzuUI.Core;

namespace GoonzuUI.Windows
{
    public sealed class UIWindowManager : MonoBehaviour
    {
        [SerializeField] private List<UIWindow> windows = new();
        [SerializeField] private bool bringToFrontOnShow = true;

        public UIEventBus Bus { get; private set; }
        private readonly Dictionary<string, UIWindow> _map = new();

        public void Initialize(UIEventBus bus)
        {
            Bus = bus;

            _map.Clear();
            foreach (var w in windows)
            {
                if (!w) continue;
                _map[w.WindowId] = w;
            }

            Bus.Subscribe<ToggleWindowEvent>(OnToggleWindow);
            Bus.Subscribe<OpenWindowEvent>(e => Open(e.WindowId));
            Bus.Subscribe<CloseWindowEvent>(e => Close(e.WindowId));
        }

        private void OnDestroy()
        {
            if (Bus == null) return;
            Bus.Unsubscribe<ToggleWindowEvent>(OnToggleWindow);
        }

        private void OnToggleWindow(ToggleWindowEvent e)
        {
            if (!_map.TryGetValue(e.WindowId, out var w) || !w) return;
            if (w.gameObject.activeSelf) Close(e.WindowId);
            else Open(e.WindowId);
        }

        public void Open(string id)
        {
            if (!_map.TryGetValue(id, out var w) || !w) return;
            w.Show();
            w.OnOpen();

            if (bringToFrontOnShow)
                w.transform.SetAsLastSibling(); // classic MMO: clicked window rises
        }

        public void Close(string id)
        {
            if (!_map.TryGetValue(id, out var w) || !w) return;
            w.OnClose();
            w.Hide();
        }
    }
}
