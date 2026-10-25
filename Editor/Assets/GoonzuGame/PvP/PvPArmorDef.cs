using System;
using GoonzuGame.Armor;

namespace GoonzuGame.PvP
{
    public class PvPArmorDef : ArmorDef
    {
        public string PvPMode { get; set; }
        public int UpgradeLevel { get; set; }
        public PvPArmorDef(string name, int defense, int durability, string rarity, string pvpType, string pveType, string theme, string pvpMode)
            : base(name, defense, durability, rarity, pvpType, pveType, theme)
        {
            PvPMode = pvpMode;
            UpgradeLevel = 0;
        }

        public void Defend()
        {
            Console.WriteLine($"Defending with {Name} (PvP Mode: {PvPMode}) for {Defense} defense.");
        }

        public void Upgrade()
        {
            UpgradeLevel++;
            Defense += 5;
            Console.WriteLine($"Upgraded {Name} to level {UpgradeLevel}. Defense is now {Defense}.");
        }

        public bool Compare(PvPArmorDef other)
        {
            return this.Defense > other.Defense;
        }

        public void Display()
        {
            Console.WriteLine($"Armor: {Name}, Defense: {Defense}, Durability: {Durability}, PvP Mode: {PvPMode}, Upgrade Level: {UpgradeLevel}, Rarity: {Rarity}, Theme: {Theme}");
        }
    }
}