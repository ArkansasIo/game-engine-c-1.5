using System;

namespace GoonzuGame.Items
{
    public class ItemDef
    {
        public string Name { get; set; }
        public string Rarity { get; set; }
        public string Theme { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
        public bool IsEquipped { get; set; }
        public ItemDef(string name, string rarity, string theme, string description, int value = 0)
        {
            Name = name;
            Rarity = rarity;
            Theme = theme;
            Description = description;
            Value = value;
            IsEquipped = false;
        }

        public void Use()
        {
            Console.WriteLine($"Used item: {Name}");
        }

        public void Equip()
        {
            IsEquipped = true;
            Console.WriteLine($"Equipped item: {Name}");
        }

        public void Unequip()
        {
            IsEquipped = false;
            Console.WriteLine($"Unequipped item: {Name}");
        }

        public int CalculateValue()
        {
            int rarityMultiplier = Rarity switch
            {
                "Common" => 1,
                "Rare" => 2,
                "Epic" => 5,
                "Legendary" => 10,
                _ => 1
            };
            return Value * rarityMultiplier;
        }

        public void Display()
        {
            Console.WriteLine($"Item: {Name}, Rarity: {Rarity}, Theme: {Theme}, Description: {Description}, Value: {Value}, Equipped: {IsEquipped}");
        }
    }
}