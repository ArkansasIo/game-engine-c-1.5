using System;
using GoonzuGame.Weapons;

namespace GoonzuGame.PvP
{
    public class PvPWeaponDef : WeaponDef
    {
        public string PvPMode { get; set; }
        public int UpgradeLevel { get; set; }
        public PvPWeaponDef(string name, int attack, int speed, string rarity, string pvpType, string pveType, string theme, string pvpMode)
            : base(name, attack, speed, rarity, pvpType, pveType, theme)
        {
            PvPMode = pvpMode;
            UpgradeLevel = 0;
        }

        public void Attack()
        {
            Console.WriteLine($"Attacking with {Name} (PvP Mode: {PvPMode}) for {Attack} damage.");
        }

        public void Upgrade()
        {
            UpgradeLevel++;
            Attack += 5;
            Console.WriteLine($"Upgraded {Name} to level {UpgradeLevel}. Attack is now {Attack}.");
        }

        public bool Compare(PvPWeaponDef other)
        {
            return this.Attack > other.Attack;
        }

        public void Display()
        {
            Console.WriteLine($"Weapon: {Name}, Attack: {Attack}, Speed: {Speed}, PvP Mode: {PvPMode}, Upgrade Level: {UpgradeLevel}, Rarity: {Rarity}, Theme: {Theme}");
        }
    }
}