using System;
using System.Collections.Generic;

namespace GoonzuGame.Data
{
    public enum SkillCategory { Combat, Gathering, Production, Social }
    public enum ItemCategory { Weapon, Armor, Consumable, Material, Tool, Mount, Pet, Quest }
    public enum QualityTier { Common, Fine, Rare, Masterwork, Perfect }

    public sealed record StatBlock(int Strength, int Agility, int Vitality, int Wisdom, int Dexterity, int Luck);

    public sealed class SkillDefinition
    {
        public string Id { get; init; } = "";
        public string Name { get; init; } = "";
        public SkillCategory Category { get; init; }
        public int BasicLevelCap { get; init; } = 30;
        public int ExperienceLevelCap { get; init; } = 70;
        public int TotalLevel => BasicLevelCap + ExperienceLevelCap;
    }

    public sealed class Recipe
    {
        public string Id { get; init; } = "";
        public string OutputItemId { get; init; } = "";
        public int Difficulty { get; init; }
        public string RequiredSkillId { get; init; } = "";
        public Dictionary<string, int> Materials { get; init; } = new();

        public double SuccessChance(int skillLevel)
        {
            if (skillLevel >= Difficulty + 5) return 1.0;
            return Math.Clamp(0.50 + ((skillLevel - Difficulty) * 0.10), 0.05, 1.0);
        }
    }

    public sealed class MarketListing
    {
        public long ListingId { get; init; }
        public string SellerId { get; init; } = "";
        public string ItemId { get; init; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int TownId { get; init; }
    }

    public static class MarketMath
    {
        public static decimal CalculatePrice(decimal basePrice, double demand, double supply, double region, double tax)
        {
            var demandMultiplier = Math.Clamp(1.0 + demand, 0.25, 4.0);
            var supplyMultiplier = Math.Clamp(1.0 - supply, 0.25, 2.0);
            var regionMultiplier = Math.Clamp(region, 0.50, 2.0);
            var taxMultiplier = Math.Clamp(1.0 + tax, 1.0, 2.0);
            return Math.Max(1m, basePrice * (decimal)(demandMultiplier * supplyMultiplier * regionMultiplier * taxMultiplier));
        }
    }

    public sealed class TownGovernment
    {
        public int TownId { get; init; }
        public string Name { get; init; } = "";
        public string MayorPlayerId { get; private set; } = "";
        public decimal Treasury { get; private set; }
        public double SalesTax { get; private set; }

        public void SetGovernment(string mayorPlayerId, double salesTax)
        {
            MayorPlayerId = mayorPlayerId;
            SalesTax = Math.Clamp(salesTax, 0.0, 0.25);
        }

        public void CollectTax(decimal amount) => Treasury += Math.Max(0m, amount);
    }

    public sealed class Election
    {
        public string Id { get; init; } = "";
        public DateTimeOffset OpensAt { get; init; }
        public DateTimeOffset ClosesAt { get; init; }
        public string Office { get; init; } = "";
        public Dictionary<string, int> Votes { get; } = new();
    }

    public sealed class GoonzuWorldState
    {
        public List<TownGovernment> Towns { get; } = new();
        public List<Election> Elections { get; } = new();
        public List<Recipe> Recipes { get; } = new();
        public List<MarketListing> Market { get; } = new();
    }
}
