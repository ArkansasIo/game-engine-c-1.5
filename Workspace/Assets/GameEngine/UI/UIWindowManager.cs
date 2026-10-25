namespace GameEngine.UI
{
    /// <summary>
    /// Manages all UI windows, handling their creation, destruction, and focus order.
    /// </summary>
    public class UIWindowManager
    {
        /// <summary>
        /// List of all registered UI windows.
        /// </summary>
        public List<UIWindow> Windows;
        /// <summary>
        /// Registers a new window with the manager.
        /// </summary>
        public void RegisterWindow(UIWindow window) { /* ... */ }
        /// <summary>
        /// Unregisters a window from the manager.
        /// </summary>
        public void UnregisterWindow(UIWindow window) { /* ... */ }
        /// <summary>
        /// Brings the specified window to the front and sets focus.
        /// </summary>
        public void FocusWindow(UIWindow window) { /* ... */ }
        /// <summary>
        /// Closes all open windows.
        /// </summary>
        public void CloseAll() { /* ... */ }
    }
}
