using System;

namespace GoonzuGame.Items
{
    [System.Serializable]
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    [System.Serializable]
    public class Item
    {
        public string Name;
        public string Type;
        public string Description;
        public int Value;
        public ItemRarity Rarity;
        public string IconPath; // Path to icon image
        public bool IsStackable;
        public int MaxStackSize = 1;

        public Item() {}

        public Item(string name, string type, string description = "", int value = 0, ItemRarity rarity = ItemRarity.Common)
        {
            Name = name;
            Type = type;
            Description = description;
            Value = value;
            Rarity = rarity;
            IsStackable = false;
        }

        public virtual void Use(GoonzuGame.Characters.Character character)
        {
            Console.WriteLine($"Used {Name}");
        }

        public virtual string GetTooltip()
        {
            return $"{Name}\n{Description}\nValue: {Value}\nRarity: {Rarity}";
        }
    }

    public class Weapon : Item
    {
        public int Damage;
        public float AttackSpeed;

        public Weapon(string name, string description, int value, ItemRarity rarity, int damage, float attackSpeed)
            : base(name, "Weapon", description, value, rarity)
        {
            Damage = damage;
            AttackSpeed = attackSpeed;
        }

        public override void Use(GoonzuGame.Characters.Character character)
        {
            // Equip weapon
            Console.WriteLine($"{character.Name} equipped {Name}");
        }
    }

    public class Armor : Item
    {
        public int Defense;

        public Armor(string name, string description, int value, ItemRarity rarity, int defense)
            : base(name, "Armor", description, value, rarity)
        {
            Defense = defense;
        }

        public override void Use(GoonzuGame.Characters.Character character)
        {
            // Equip armor
            Console.WriteLine($"{character.Name} equipped {Name}");
        }
    }

    public class Consumable : Item
    {
        public int HealAmount;

        public Consumable(string name, string description, int value, ItemRarity rarity, int healAmount)
            : base(name, "Consumable", description, value, rarity)
        {
            HealAmount = healAmount;
            IsStackable = true;
            MaxStackSize = 99;
        }

        public override void Use(GoonzuGame.Characters.Character character)
        {
            character.Health = Math.Min(character.Health + HealAmount, character.MaxHealth);
            Console.WriteLine($"{character.Name} used {Name} and healed {HealAmount} HP");
        }
    }
}
