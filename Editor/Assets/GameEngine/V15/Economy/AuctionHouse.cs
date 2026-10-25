using System;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine.V15.Economy
{
    public sealed record AuctionListing(string Id, string SellerId, string ItemId, int Quantity, long UnitPrice);

    public sealed class AuctionHouse
    {
        private readonly Dictionary<string, AuctionListing> listings = new();
        public AuctionListing List(string id, string sellerId, string itemId, int quantity, long unitPrice)
        {
            if (quantity <= 0 || unitPrice < 0) throw new ArgumentOutOfRangeException();
            var listing = new AuctionListing(id, sellerId, itemId, quantity, unitPrice);
            listings.Add(id, listing);
            return listing;
        }
        public IReadOnlyList<AuctionListing> Search(string itemId) => listings.Values.Where(x => x.ItemId == itemId).OrderBy(x => x.UnitPrice).ToList();
        public bool Cancel(string id, string sellerId)
        {
            return listings.TryGetValue(id, out var listing) && listing.SellerId == sellerId && listings.Remove(id);
        }
        public bool TryGet(string id, out AuctionListing listing) => listings.TryGetValue(id, out listing!);
    }
}
