using System;
using System.Collections.Generic;

namespace GoonzuGame.Mobs
{
    public class MobDef
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public List<string> Abilities { get; set; }
        public string Theme { get; set; }
        public MobDef(string name, int level, int health, int attack, int defense, List<string> abilities, string theme)
        {
            Name = name;
            Level = level;
            Health = health;
            Attack = attack;
            Defense = defense;
            Abilities = abilities;
            Theme = theme;
        }
    }
}