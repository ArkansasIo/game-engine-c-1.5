using System;

namespace GoonzuGame.Core
{
    public class Item
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Rarity { get; set; }
        public int Value { get; set; }
        public bool IsConsumable { get; set; }

        public void Use(Character user)
        {
            if (IsConsumable)
            {
                user.Health += 20;
                Console.WriteLine($"{user.Name} used {Name} and restored health.");
            }
            else
            {
                Console.WriteLine($"{user.Name} used {Name}.");
            }
        }
    }
}
