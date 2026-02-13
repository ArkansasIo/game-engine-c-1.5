namespace GameEngine
{
    /// <summary>
    /// Manages the player-driven market, listings, and transactions.
    /// </summary>
    public class MarketManager
    {
        private class MarketItem
        {
            public string Seller = string.Empty;
            public string Item = string.Empty;
            public int Price;
        }

        private List<MarketItem> market = new();

        public void ListItem(string sellerName, string itemName, int price)
        {
            market.Add(new MarketItem { Seller = sellerName, Item = itemName, Price = price });
            System.Console.WriteLine($"{itemName} listed by {sellerName} for {price} gold.");
        }

        public void BuyItem(string buyerName, string itemName, ref int buyerGold, List<string> buyerInventory)
        {
            var item = market.Find(i => i.Item == itemName);
            if (item == null)
            {
                System.Console.WriteLine($"{itemName} is not available in the market.");
                return;
            }
            if (buyerGold < item.Price)
            {
                System.Console.WriteLine($"{buyerName} does not have enough gold to buy {itemName}.");
                return;
            }
            buyerGold -= item.Price;
            buyerInventory.Add(item.Item);
            market.Remove(item);
            System.Console.WriteLine($"{buyerName} bought {item.Item} for {item.Price} gold from {item.Seller}.");
        }

        public void ListMarket()
        {
            if (market.Count == 0)
            {
                System.Console.WriteLine("No items in the market.");
                return;
            }
            foreach (var item in market)
                System.Console.WriteLine($"{item.Item} by {item.Seller} for {item.Price} gold");
        }
    }
}
