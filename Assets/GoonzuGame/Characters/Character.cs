using System;

namespace GoonzuGame.Characters
{
    public class Character
    {
        public string Name { get; set; }
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
        }

        public void Attack(Character target)
        {
            int damage = 10 + Level;
            target.Health -= damage;
            Console.WriteLine($"{Name} attacks {target.Name} for {damage} damage.");
        }

        public void Move(string direction)
        {
            Console.WriteLine($"{Name} moves {direction}.");
        }

        public void PickUpItem(Item item)
        {
            Inventory.Add(item);
            Console.WriteLine($"{Name} picked up {item.Name}.");
        }

        public void CompleteQuest(Quest quest)
        {
            quest.Complete();
            ActiveQuests.Remove(quest);
            Experience += 100;
            Gold += 50;
            Console.WriteLine($"{Name} completed quest: {quest.Title}.");
        }

        public void LevelUp()
        {
            if (Experience >= Level * 100)
            {
                Level++;
                Experience = 0;
                Health += 20;
                Mana += 10;
                Console.WriteLine($"{Name} leveled up to {Level}!");
            }
        }
    }
}
