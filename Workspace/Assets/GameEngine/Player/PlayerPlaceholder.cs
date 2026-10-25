using System.Collections.Generic;
namespace GameEngine.Player
{
    public class Player
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Gold { get; set; }
        public List<string> Inventory { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }

        public Player(string name)
        {
            Name = name;
            Level = 1;
            Experience = 0;
            Gold = 0;
            Inventory = new List<string>();
            Health = 100;
            Mana = 50;
        }

        public void Move(string direction)
        {
            System.Console.WriteLine($"{Name} moves {direction}.");
        }

        public void Attack(string target)
        {
            System.Console.WriteLine($"{Name} attacks {target}.");
        }

        public void AddItem(string item)
        {
            Inventory.Add(item);
            System.Console.WriteLine($"{Name} received item: {item}.");
        }

        public void UseItem(string item)
        {
            if (Inventory.Contains(item))
            {
                Inventory.Remove(item);
                System.Console.WriteLine($"{Name} used item: {item}.");
            }
            else
            {
                System.Console.WriteLine($"{Name} does not have item: {item}.");
            }
        }
    }
}
