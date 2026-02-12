using System;
using GoonzuGame.Items;

namespace GoonzuGame.Equipment
{
    public class EquipmentDef : Item
    {
        public string Slot { get; set; }
        public int Power { get; set; }
        public int Durability { get; set; }
        public string Theme { get; set; }
        public EquipmentDef(string name, string slot, int power, int durability, string rarity, string theme)
            : base(name, rarity)
        {
            Slot = slot;
            Power = power;
            Durability = durability;
            Theme = theme;
        }
    }
}