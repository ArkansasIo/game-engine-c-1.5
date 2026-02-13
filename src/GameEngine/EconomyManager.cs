namespace GameEngine
{
    /// <summary>
    /// Manages the in-game economy, prices, and transactions.
    /// </summary>
    public class EconomyManager
    {
        public void UpdatePrices()
        {
            System.Console.WriteLine("Economy prices updated (stub).");
        }
        public void RecordTransaction(string buyer, string seller, string item, int price)
        {
            System.Console.WriteLine($"Transaction: {buyer} bought {item} from {seller} for {price} gold.");
        }
    }
}
