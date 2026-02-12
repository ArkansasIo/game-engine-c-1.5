using System;

namespace GoonzuGame.Items
{
    public class Item
    {
        public string Name { get; set; }
        public string Type { get; set; }
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
