namespace GameEngine.UI
{
    /// <summary>
    /// UI window for displaying and managing the player's inventory.
    /// </summary>
    public class InventoryWindow : UIWindow
    {
        /// <summary>
        /// List of items currently in the player's inventory.
        /// </summary>
        public List<string> Items;
        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        public void AddItem(string item) { /* ... */ }
        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        public void RemoveItem(string item) { /* ... */ }
        /// <summary>
        /// Selects an item for use, equip, or inspection.
        /// </summary>
        public void SelectItem(string item) { /* ... */ }
    }
}
