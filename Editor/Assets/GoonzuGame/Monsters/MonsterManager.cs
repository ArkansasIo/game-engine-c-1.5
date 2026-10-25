using System;
using GoonzuGame.Core;

namespace GoonzuGame.Monsters
{
    using System.Collections.Generic;
    // using GoonzuGame.Characters;
    // using GoonzuGame.Items;

    public class Monster
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public bool IsBoss { get; set; }
        public List<Item> LootTable { get; set; }
        public Monster(string name, bool isBoss = false)
        {
            Name = name;
            Health = isBoss ? 500 : 100;
            IsBoss = isBoss;
            LootTable = new List<Item>();
        }
        public void Attack(Character target)
        {
            int damage = IsBoss ? 50 : 10;
            target.Health -= damage;
            Console.WriteLine($"{Name} attacks {target.Name} for {damage} damage.");
        }
    }

    public class MonsterManager
    {
        public List<Monster> Monsters { get; set; }

        public MonsterManager()
        {
            Monsters = new List<Monster>();
        }

        public void SpawnMonster(string monsterType, bool isBoss = false)
        {
            var monster = new Monster(monsterType, isBoss);
            Monsters.Add(monster);
            Console.WriteLine($"Spawned {(isBoss ? "Boss" : "Monster")}: {monsterType}");
        }

        public void StartBossFight(string bossName, Character player)
        {
            var boss = Monsters.Find(m => m.Name == bossName && m.IsBoss);
            if (boss != null)
            {
                Console.WriteLine($"Boss fight started: {bossName}");
                while (boss.Health > 0 && player.Health > 0)
                {
                    boss.Attack(player);
                    if (player.Health > 0) player.Attack(boss);
                }
                string winner = boss.Health > 0 ? boss.Name : player.Name;
                Console.WriteLine($"Boss fight ended. Winner: {winner}");
            }
        }

        public void DropLoot(string monsterType, Character player)
        {
            var monster = Monsters.Find(m => m.Name == monsterType);
            if (monster != null && monster.LootTable.Count > 0)
            {
                var loot = monster.LootTable[0];
                player.PickUpItem(loot);
                Console.WriteLine($"{player.Name} received loot: {loot.Name}");
            }
        }
    }
}
