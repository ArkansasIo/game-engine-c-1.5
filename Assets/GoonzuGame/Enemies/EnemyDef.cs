using System;
using System.Collections.Generic;

namespace GoonzuGame.Enemies
{
    public class EnemyDef
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public List<string> Abilities { get; set; }
        public string Theme { get; set; }

        // DnD 5e stats
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        public EnemyDef(string name, int level, int health, int attack, int defense, List<string> abilities, string theme,
            int str, int dex, int con, int intel, int wis, int cha)
        {
            Name = name;
            Level = level;
            Health = health;
            Attack = attack;
            Defense = defense;
            Abilities = abilities;
            Theme = theme;
            Strength = str;
            Dexterity = dex;
            Constitution = con;
            Intelligence = intel;
            Wisdom = wis;
            Charisma = cha;
        }

        public int AbilityModifier(int score) => (score - 10) / 2;

        public int SavingThrow(string stat)
        {
            int mod = stat switch
            {
                "STR" => AbilityModifier(Strength),
                "DEX" => AbilityModifier(Dexterity),
                "CON" => AbilityModifier(Constitution),
                "INT" => AbilityModifier(Intelligence),
                "WIS" => AbilityModifier(Wisdom),
                "CHA" => AbilityModifier(Charisma),
                _ => 0
            };
            return RollD20() + mod;
        }

        public int AttackRoll()
        {
            return RollD20() + AbilityModifier(Strength);
        }

        public int Initiative()
        {
            return RollD20() + AbilityModifier(Dexterity);
        }

        public int DamageRoll(int diceCount, int diceType)
        {
            int total = 0;
            for (int i = 0; i < diceCount; i++)
                total += RollDice(diceType);
            return total + AbilityModifier(Strength);
        }

        public int RollD20() => RollDice(20);
        public int RollDice(int sides)
        {
            return new Random().Next(1, sides + 1);
        }

        public void DisplayStats()
        {
            Console.WriteLine($"Enemy: {Name}, Level: {Level}, HP: {Health}, STR: {Strength}, DEX: {Dexterity}, CON: {Constitution}, INT: {Intelligence}, WIS: {Wisdom}, CHA: {Charisma}");
        }
    }
}