using System;
using GoonzuGame.Armor;

namespace GoonzuGame.PvE
{
    public class PvEArmorDef : ArmorDef
    {
        public string PvEMode { get; set; }
        public int UpgradeLevel { get; set; }
        public PvEArmorDef(string name, int defense, int durability, string rarity, string pvpType, string pveType, string theme, string pveMode)
            : base(name, defense, durability, rarity, pvpType, pveType, theme)
        {
            PvEMode = pveMode;
            UpgradeLevel = 0;
        }

        public void Defend()
        {
            Console.WriteLine($"Defending with {Name} (PvE Mode: {PvEMode}) for {Defense} defense.");
        }

        public void Upgrade()
        {
            UpgradeLevel++;
            Defense += 5;
            Console.WriteLine($"Upgraded {Name} to level {UpgradeLevel}. Defense is now {Defense}.");
        }

        public bool Compare(PvEArmorDef other)
        {
            return this.Defense > other.Defense;
        }

        public void Display()
        {
            Console.WriteLine($"Armor: {Name}, Defense: {Defense}, Durability: {Durability}, PvE Mode: {PvEMode}, Upgrade Level: {UpgradeLevel}, Rarity: {Rarity}, Theme: {Theme}");
        }
    }
}