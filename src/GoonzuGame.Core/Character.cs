using System;
using System.Collections.Generic;

namespace GoonzuGame.Core
{
    public class Character
    {
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        public int Experience { get; set; }
        public int Gold { get; set; }
        public List<Item> Inventory { get; set; }
        public List<Quest> ActiveQuests { get; set; }

        public Character()
        {
            Inventory = new List<Item>();
            ActiveQuests = new List<Quest>();
            Level = 1;
            Health = 100;
            Mana = 50;
            Experience = 0;
            Gold = 0;
            Name = string.Empty;
        }

        public Character(string name) : this()
        {
            Name = name;
        }

        public void Attack(Character target)
        {
            int damage = 10 + Level;
            target.Health -= damage;
            Console.WriteLine($"{Name} attacks {target.Name} for {damage} damage.");
        }
    }
}
