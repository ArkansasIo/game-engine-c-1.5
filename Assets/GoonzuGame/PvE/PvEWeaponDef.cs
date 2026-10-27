using System;
using GoonzuGame.Weapons;

namespace GoonzuGame.PvE
{
    public class PvEWeaponDef : WeaponDef
    {
        public string PvEMode { get; set; }
        public int UpgradeLevel { get; set; }
        public PvEWeaponDef(string name, int attack, int speed, string rarity, string pvpType, string pveType, string theme, string pveMode)
            : base(name, attack, speed, ParseRarity(rarity), pvpType, pveType, theme)
        {
            PvEMode = pveMode;
            UpgradeLevel = 0;
        }

        private static GoonzuGame.Items.ItemRarity ParseRarity(string rarity) => rarity switch
        {
            "Common" => GoonzuGame.Items.ItemRarity.Common,
            "Uncommon" => GoonzuGame.Items.ItemRarity.Uncommon,
            "Rare" => GoonzuGame.Items.ItemRarity.Rare,
            "Epic" => GoonzuGame.Items.ItemRarity.Epic,
            "Legendary" => GoonzuGame.Items.ItemRarity.Legendary,
            _ => GoonzuGame.Items.ItemRarity.Common,
        };

        public void DoAttack()
        {
            Console.WriteLine($"Attacking with {Name} (PvE Mode: {PvEMode}) for {Damage} damage.");
        }

        public void Upgrade()
        {
            UpgradeLevel++;
            Damage += 5;
            Console.WriteLine($"Upgraded {Name} to level {UpgradeLevel}. Attack is now {Damage}.");
        }

        public bool Compare(PvEWeaponDef other)
        {
            return this.Damage > other.Damage;
        }

        public new void Display()
        {
            Console.WriteLine($"Weapon: {Name}, Attack: {Damage}, Speed: {AttackSpeed}, PvE Mode: {PvEMode}, Upgrade Level: {UpgradeLevel}, Rarity: {Rarity}, Theme: {Theme}");
        }
    }
}