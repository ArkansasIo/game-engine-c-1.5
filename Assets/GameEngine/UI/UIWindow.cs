namespace GameEngine.UI
{
    /// <summary>
    /// Represents a generic UI window. Handles opening, closing, and focus logic for UI panels.
    /// </summary>
    public class UIWindow
    {
        /// <summary>
        /// Indicates whether the window is currently open.
        /// </summary>
        public bool IsOpen;
        /// <summary>
        /// Opens the window and makes it visible.
        /// </summary>
        public void Open() { /* ... */ }
        /// <summary>
        /// Closes the window and hides it from view.
        /// </summary>
        public void Close() { /* ... */ }
        /// <summary>
        /// Sets focus to this window, bringing it to the front.
        /// </summary>
        public void Focus() { /* ... */ }
    }
}
