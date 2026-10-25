using System.Text;
using UnityEngine;
using GoonzuUI.Core;

namespace GoonzuUI.Chat
{
    public sealed class ChatController : MonoBehaviour
    {
        [SerializeField] private ChatView view;

        private UIEventBus _bus;
        private ChatModel _model = new();

        private void OnDestroy()
        {
            if (_bus == null) return;
            _bus.Unsubscribe<ChatAppendEvent>(OnAppend);
        }

        public void Initialize(UIEventBus bus)
        {
            _bus = bus;
            _bus.Subscribe<ChatAppendEvent>(OnAppend);
        }

        public void OnSendClicked()
        {
            var msg = view.ConsumeInput()?.Trim();
            if (string.IsNullOrEmpty(msg)) return;

            _bus.Publish(new ChatSendEvent(view.SelectedChannel, msg));
            view.ClearInput();
        }

        private void OnAppend(ChatAppendEvent e)
        {
            var line = string.IsNullOrWhiteSpace(e.From)
                ? e.Message
                : $"[{e.Channel}] {e.From}: {e.Message}";

            _model.Append(e.Channel, line);

            // Re-render active channel like older MMOs (simple but effective)
            var lines = _model.GetLines(view.SelectedChannel);
            var sb = new StringBuilder(lines.Count * 32);
            foreach (var l in lines) sb.AppendLine(l);
            view.SetOutput(sb.ToString());
        }
    }
}
