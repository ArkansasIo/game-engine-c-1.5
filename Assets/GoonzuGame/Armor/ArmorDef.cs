using System;
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
        public ArmorDef(string name, int defense, int durability, string rarity, string pvpType, string pveType, string theme)
            : base(name, rarity)
        {
            Defense = defense;
            Durability = durability;
            PvPType = pvpType;
            PvEType = pveType;
            Theme = theme;
        }
    }
}