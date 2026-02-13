using System;
using GoonzuGame.Items;
using GoonzuGame.Items;

namespace GoonzuGame.Weapons
{
    public class WeaponDef : Item
    {
        public int Attack { get; set; }
        public int Speed { get; set; }
        public string PvPType { get; set; }
        public string PvEType { get; set; }
        public string Theme { get; set; }
        public bool IsEquipped { get; set; }
        public string Proficiency { get; set; }
        public WeaponDef(string name, int attack, int speed, string rarity, string pvpType, string pveType, string theme, string proficiency = "")
            : base(name, rarity)
        {
            Attack = attack;
            Speed = speed;
            PvPType = pvpType;
            PvEType = pveType;
            Theme = theme;
            Proficiency = proficiency;
            IsEquipped = false;
        }

        public void Equip()
        {
            IsEquipped = true;
            Console.WriteLine($"Equipped weapon: {Name}");
        }

        public void Unequip()
        {
            IsEquipped = false;
            Console.WriteLine($"Unequipped weapon: {Name}");
        }

        public bool CheckProficiency(string characterProficiency)
        {
            return Proficiency == characterProficiency;
        }

        public int AttackRoll(int strMod)
        {
            return RollD20() + strMod;
        }

        public int DamageRoll(int diceCount, int diceType, int strMod)
        {
            int total = 0;
            for (int i = 0; i < diceCount; i++)
                total += RollDice(diceType);
            return total + strMod;
        }

        public int RollD20() => RollDice(20);
        public int RollDice(int sides)
        {
            return new Random().Next(1, sides + 1);
        }

        public void Display()
        {
            Console.WriteLine($"Weapon: {Name}, Attack: {Attack}, Speed: {Speed}, PvPType: {PvPType}, PvEType: {PvEType}, Theme: {Theme}, Proficiency: {Proficiency}, Equipped: {IsEquipped}");
        }
    }
}