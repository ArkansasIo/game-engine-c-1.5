namespace GameEngine
{
    /// <summary>
    /// Manages the player-driven market, listings, and transactions.
    /// </summary>
    public class MarketManager
    {
        public void ListItem(string sellerName, string itemName, int price)
        {
            System.Console.WriteLine($"{itemName} listed by {sellerName} for {price} gold.");
        }
        public void BuyItem(string buyerName, string itemName)
        {
            System.Console.WriteLine($"{buyerName} bought {itemName} (stub).");
        }
        public void ListMarket()
        {
            System.Console.WriteLine("Listing all market items (stub).");
        }
    }
}
