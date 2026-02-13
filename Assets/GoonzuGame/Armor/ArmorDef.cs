using System;
using GoonzuGame.Core;
using GoonzuGame.Items;

namespace GoonzuGame.Armor
{
    public class ArmorDef : Item
    {
        public int Defense { get; set; }
        public int Durability { get; set; }
        public string PvPType { get; set; }
        public string PvEType { get; set; }
        public string Theme { get; set; }
        public bool IsEquipped { get; set; }
        public string Proficiency { get; set; }
        public ArmorDef(string name, int defense, int durability, string rarity, string pvpType, string pveType, string theme, string proficiency = "")
            : base(name, rarity)
        {
            Defense = defense;
            Durability = durability;
            PvPType = pvpType;
            PvEType = pveType;
            Theme = theme;
            Proficiency = proficiency;
            IsEquipped = false;
        }

        public void Equip()
        {
            IsEquipped = true;
            Console.WriteLine($"Equipped armor: {Name}");
        }

        public void Unequip()
        {
            IsEquipped = false;
            Console.WriteLine($"Unequipped armor: {Name}");
        }

        public bool CheckProficiency(string characterProficiency)
        {
            return Proficiency == characterProficiency;
        }

        public int CalculateAC(int dexMod)
        {
            // DnD 5e: AC = base + Dex mod (if allowed)
            return Defense + dexMod;
        }

        public void Display()
        {
            Console.WriteLine($"Armor: {Name}, Defense: {Defense}, Durability: {Durability}, PvPType: {PvPType}, PvEType: {PvEType}, Theme: {Theme}, Proficiency: {Proficiency}, Equipped: {IsEquipped}");
        }
    }
}