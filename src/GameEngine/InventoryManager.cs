namespace GameEngine
{
    /// <summary>
    /// Manages inventory logic for all players.
    /// </summary>
    public class InventoryManager
    {
        public void AddItem(string playerName, string itemName)
        {
            System.Console.WriteLine($"{itemName} added to {playerName}'s inventory.");
        }
        public void RemoveItem(string playerName, string itemName)
        {
            System.Console.WriteLine($"{itemName} removed from {playerName}'s inventory.");
        }
        public void ListInventory(string playerName)
        {
            System.Console.WriteLine($"Listing inventory for {playerName} (stub).");
        }
    }
}
