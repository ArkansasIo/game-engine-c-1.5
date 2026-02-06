using System;
using System.Collections.Generic;

namespace GoonzuUI.Core
{
    // Lightweight message bus for UI decoupling (no singletons required, but convenient to keep one instance).
    public sealed class UIEventBus
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();

        public void Subscribe<T>(Action<T> handler)
        {
            var t = typeof(T);
            if (_handlers.TryGetValue(t, out var d)) _handlers[t] = Delegate.Combine(d, handler);
            else _handlers[t] = handler;
        }

        public void Unsubscribe<T>(Action<T> handler)
        {
            var t = typeof(T);
            if (_handlers.TryGetValue(t, out var d))
            {
                var nd = Delegate.Remove(d, handler);
                if (nd == null) _handlers.Remove(t);
                else _handlers[t] = nd;
            }
        }

        public void Publish<T>(T evt)
        {
            if (_handlers.TryGetValue(typeof(T), out var d))
                (d as Action<T>)?.Invoke(evt);
        }
    }

    // Events
    public readonly record struct ToggleWindowEvent(string WindowId);
    public readonly record struct OpenWindowEvent(string WindowId);
    public readonly record struct CloseWindowEvent(string WindowId);

    public readonly record struct ChatSendEvent(string Channel, string Message);
    public readonly record struct ChatAppendEvent(string Channel, string Message, string From);

    public readonly record struct HotbarUseSlotEvent(int SlotIndex);
}
