using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame.Trade
{
    [System.Serializable]
    public class TradeOffer
    {
        public GoonzuGame.Items.Item Item;
        public int Quantity;
        public int Price;

        public TradeOffer(GoonzuGame.Items.Item item, int quantity, int price)
        {
            Item = item;
            Quantity = quantity;
            Price = price;
        }
    }

    public class TradeManager : MonoBehaviour
    {
        public static TradeManager Instance { get; private set; }

        public int PlayerGold = 1000;
        public List<TradeOffer> ShopInventory = new List<TradeOffer>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeShop();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeShop()
        {
            // Add sample items to shop
            var sword = new GoonzuGame.Items.Weapon("Iron Sword", "A sturdy sword", 50, GoonzuGame.Items.ItemRarity.Common, 10, 1.0f);
            ShopInventory.Add(new TradeOffer(sword, 1, 100));

            var potion = new GoonzuGame.Items.Consumable("Health Potion", "Restores health", 10, GoonzuGame.Items.ItemRarity.Common, 20);
            ShopInventory.Add(new TradeOffer(potion, 10, 25));
        }

        public void StartTrade()
        {
            Debug.Log("Trade started");
            // Show trade UI
            GoonzuGame.UI.UIManager.Instance.ShowWindow("Trade");
        }

        public void CompleteTrade()
        {
            Debug.Log("Trade completed");
            // Hide trade UI
            GoonzuGame.UI.UIManager.Instance.HideWindow("Trade");
        }

        public bool BuyItem(TradeOffer offer)
        {
            if (PlayerGold >= offer.Price)
            {
                PlayerGold -= offer.Price;
                GoonzuGame.Inventory.InventoryManager.Instance.AddItem(offer.Item, offer.Quantity);
                Debug.Log($"Bought {offer.Item.Name} for {offer.Price} gold");
                return true;
            }
            else
            {
                Debug.Log("Not enough gold");
                return false;
            }
        }

        public bool SellItem(GoonzuGame.Items.Item item, int quantity, int price)
        {
            if (GoonzuGame.Inventory.InventoryManager.Instance.RemoveItem(item, quantity))
            {
                PlayerGold += price;
                Debug.Log($"Sold {item.Name} for {price} gold");
                return true;
            }
            return false;
        }

        public void AddGold(int amount)
        {
            PlayerGold += amount;
            Debug.Log($"Gained {amount} gold. Total: {PlayerGold}");
        }

        public void RemoveGold(int amount)
        {
            PlayerGold = Mathf.Max(0, PlayerGold - amount);
            Debug.Log($"Lost {amount} gold. Total: {PlayerGold}");
        }

        public List<TradeOffer> GetShopItems()
        {
            return ShopInventory;
        }
    }
}
