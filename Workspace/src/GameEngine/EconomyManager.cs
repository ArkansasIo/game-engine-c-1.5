namespace GameEngine
{
    /// <summary>
    /// Manages the in-game economy, prices, and transactions.
    /// </summary>
    public class EconomyManager
    {
        private Dictionary<string, int> itemPrices = new();
        private List<string> transactionHistory = new();

        public void UpdatePrices()
        {
            // Example: Randomly adjust prices for demonstration
            var rand = new System.Random();
            foreach (var item in itemPrices.Keys)
            {
                int change = rand.Next(-5, 6); // -5 to +5
                itemPrices[item] = System.Math.Max(1, itemPrices[item] + change);
            }
            System.Console.WriteLine("Economy prices updated.");
        }

        public void SetPrice(string item, int price)
        {
            itemPrices[item] = price;
            System.Console.WriteLine($"Set price for {item}: {price} gold");
        }

        public int GetPrice(string item)
        {
            return itemPrices.TryGetValue(item, out var price) ? price : -1;
        }

        public void RecordTransaction(string buyer, string seller, string item, int price)
        {
            string record = $"{buyer} bought {item} from {seller} for {price} gold.";
            transactionHistory.Add(record);
            System.Console.WriteLine(record);
        }

        public void ShowTransactionHistory()
        {
            if (transactionHistory.Count == 0)
            {
                System.Console.WriteLine("No transactions recorded.");
                return;
            }
            foreach (var record in transactionHistory)
                System.Console.WriteLine(record);
        }
    }
}
