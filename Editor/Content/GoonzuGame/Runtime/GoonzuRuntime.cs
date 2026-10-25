using System;
using System.Collections.Generic;

namespace GoonzuGame.Runtime;

public enum SkillFamily { Combat, Gathering, Manufacturing, Social, Companion }

public sealed record PlayerStats(int Strength, int Agility, int Vitality, int Wisdom, int Dexterity, int Luck);

public sealed class SkillProgress
{
    public string Id { get; init; } = "";
    public SkillFamily Family { get; init; }
    public int BasicLevel { get; private set; }
    public int ExperienceLevel { get; private set; }
    public int TotalLevel => Math.Clamp(BasicLevel + ExperienceLevel, 0, 100);

    public void AddBasicLevel(int amount) => BasicLevel = Math.Clamp(BasicLevel + amount, 0, 30);
    public void AddExperience(int amount)
    {
        if (amount <= 0) return;
        var gained = Math.Max(1, amount / 100);
        ExperienceLevel = Math.Clamp(ExperienceLevel + gained, 0, 70);
    }
}

public sealed record Recipe(
    string Id,
    string OutputItem,
    int OutputQuantity,
    IReadOnlyDictionary<string, int> Inputs,
    string Profession,
    int RequiredSkill,
    double BaseSuccess);

public sealed record MarketListing(
    long Id,
    long SellerId,
    string ItemId,
    long Quantity,
    long UnitPrice,
    string TownId);

public sealed class GoonzuEconomyRuntime
{
    public List<MarketListing> Listings { get; } = new();
    public Dictionary<string, SkillProgress> Skills { get; } = new(StringComparer.OrdinalIgnoreCase);

    public double CalculateCraftChance(Recipe recipe, int skill, int artisanBonus = 0, int blessingBonus = 0)
    {
        var skillBonus = Math.Clamp((skill - recipe.RequiredSkill) * 0.01, -0.50, 0.35);
        return Math.Clamp(recipe.BaseSuccess + skillBonus + artisanBonus * 0.01 + blessingBonus * 0.01, 0.05, 0.99);
    }

    public decimal AveragePrice(string itemId)
    {
        var matching = Listings.FindAll(x => x.ItemId.Equals(itemId, StringComparison.OrdinalIgnoreCase) && x.Quantity > 0);
        if (matching.Count == 0) return 0m;
        long quantity = 0;
        decimal value = 0m;
        foreach (var listing in matching)
        {
            quantity += listing.Quantity;
            value += listing.Quantity * listing.UnitPrice;
        }
        return quantity == 0 ? 0m : value / quantity;
    }
}
