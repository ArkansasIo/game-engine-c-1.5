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
        public bool IsEquipped { get; set; }
        public EquipmentDef(string name, string slot, int power, int durability, string rarity, string theme)
            : base(name, rarity)
        {
            Slot = slot;
            Power = power;
            Durability = durability;
            Theme = theme;
            IsEquipped = false;
        }

        public void Equip()
        {
            IsEquipped = true;
            Console.WriteLine($"Equipped equipment: {Name} in slot {Slot}");
        }

        public void Unequip()
        {
            IsEquipped = false;
            Console.WriteLine($"Unequipped equipment: {Name} from slot {Slot}");
        }

        public bool CheckSlot(string slot)
        {
            return Slot == slot;
        }

        public void Repair(int amount)
        {
            Durability += amount;
            Console.WriteLine($"Repaired {Name} by {amount}. Durability is now {Durability}.");
        }

        public void Upgrade(int amount)
        {
            Power += amount;
            Console.WriteLine($"Upgraded {Name} by {amount}. Power is now {Power}.");
        }

        public bool IsBroken()
        {
            return Durability <= 0;
        }

        public int CalculateValue()
        {
            return Power * Durability;
        }

        public void Display()
        {
            Console.WriteLine($"Equipment: {Name}, Slot: {Slot}, Power: {Power}, Durability: {Durability}, Theme: {Theme}, Equipped: {IsEquipped}");
        }
    }
}