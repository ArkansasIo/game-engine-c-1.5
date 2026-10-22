using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArkansasIo.GoonzuGameEngine;

[Serializable]
public struct GoonzuMarketListing
{
    public long id;
    public long sellerId;
    public string itemId;
    public long quantity;
    public long unitPrice;
    public string townId;
}

[Serializable]
public struct GoonzuRecipe
{
    public string id;
    public string outputItem;
    public int outputQuantity;
    public string profession;
    public int requiredSkill;
    [Range(0f, 1f)] public float baseSuccess;
}

public sealed class GoonzuEconomyService : MonoBehaviour
{
    [SerializeField] private List<GoonzuMarketListing> listings = new();

    public IReadOnlyList<GoonzuMarketListing> Listings => listings;

    public void AddListing(GoonzuMarketListing listing)
    {
        if (listing.quantity <= 0 || listing.unitPrice < 0 || string.IsNullOrWhiteSpace(listing.itemId))
            throw new ArgumentException("Invalid market listing.");
        listings.Add(listing);
    }

    public decimal AveragePrice(string itemId)
    {
        long quantity = 0;
        decimal value = 0m;
        foreach (var listing in listings)
        {
            if (!string.Equals(listing.itemId, itemId, StringComparison.OrdinalIgnoreCase) || listing.quantity <= 0)
                continue;
            quantity += listing.quantity;
            value += (decimal)listing.quantity * listing.unitPrice;
        }
        return quantity == 0 ? 0m : value / quantity;
    }

    public static float CalculateCraftChance(GoonzuRecipe recipe, int skill, int artisanBonus = 0, int blessingBonus = 0)
    {
        var skillBonus = Mathf.Clamp((skill - recipe.requiredSkill) * 0.01f, -0.50f, 0.35f);
        return Mathf.Clamp(recipe.baseSuccess + skillBonus + artisanBonus * 0.01f + blessingBonus * 0.01f, 0.05f, 0.99f);
    }
}
