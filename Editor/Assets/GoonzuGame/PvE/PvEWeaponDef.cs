using System;
using GoonzuGame.Weapons;

namespace GoonzuGame.PvE
{
    public class PvEWeaponDef : WeaponDef
    {
        public string PvEMode { get; set; }
        public int UpgradeLevel { get; set; }
        public PvEWeaponDef(string name, int attack, int speed, string rarity, string pvpType, string pveType, string theme, string pveMode)
            : base(name, attack, speed, rarity, pvpType, pveType, theme)
        {
            PvEMode = pveMode;
            UpgradeLevel = 0;
        }

        public void Attack()
        {
            Console.WriteLine($"Attacking with {Name} (PvE Mode: {PvEMode}) for {Attack} damage.");
        }

        public void Upgrade()
        {
            UpgradeLevel++;
            Attack += 5;
            Console.WriteLine($"Upgraded {Name} to level {UpgradeLevel}. Attack is now {Attack}.");
        }

        public bool Compare(PvEWeaponDef other)
        {
            return this.Attack > other.Attack;
        }

        public void Display()
        {
            Console.WriteLine($"Weapon: {Name}, Attack: {Attack}, Speed: {Speed}, PvE Mode: {PvEMode}, Upgrade Level: {UpgradeLevel}, Rarity: {Rarity}, Theme: {Theme}");
        }
    }
}