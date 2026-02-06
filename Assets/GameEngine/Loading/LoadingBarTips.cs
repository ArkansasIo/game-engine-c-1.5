namespace GameEngine.Loading
{
    /// <summary>
    /// Displays a loading bar and random gameplay tips during scene transitions.
    /// </summary>
    public class LoadingBarTips
    {
        /// <summary>
        /// List of tips to display during loading.
        /// </summary>
        public List<string> Tips;
        /// <summary>
        /// Shows the loading bar and a random tip.
        /// </summary>
        public void ShowLoadingBar() { /* ... */ }
        /// <summary>
        /// Hides the loading bar when loading is complete.
        /// </summary>
        public void HideLoadingBar() { /* ... */ }
        /// <summary>
        /// Selects and displays a random tip from the list.
        /// </summary>
        public void ShowRandomTip() { /* ... */ }
    }
}
