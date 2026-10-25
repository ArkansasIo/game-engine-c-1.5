using System;
using System.Collections.Generic;

namespace GoonzuGame.Characters
{
    [System.Serializable]
    public enum CharacterClass
    {
        Warrior,
        Mage,
        Rogue,
        Cleric
    }

    [System.Serializable]
    public class Stats
    {
        public int Strength;
        public int Dexterity;
        public int Intelligence;
        public int Wisdom;
        public int Constitution;
        public int Charisma;

        public Stats(int str = 10, int dex = 10, int intel = 10, int wis = 10, int con = 10, int cha = 10)
        {
            Strength = str;
            Dexterity = dex;
            Intelligence = intel;
            Wisdom = wis;
            Constitution = con;
            Charisma = cha;
        }
    }

    [System.Serializable]
    public struct Vector3
    {
        public float X, Y, Z;
        public Vector3(float x, float y, float z) { X = x; Y = y; Z = z; }
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public override string ToString() => $"({X}, {Y}, {Z})";
    }

    public class Character
    {
        public string Name = "";
        public int Level = 1;
        public int Experience = 0;
        public int ExperienceToNextLevel = 100;
        public int Health = 100;
        public int MaxHealth = 100;
        public int Mana = 50;
        public int MaxMana = 50;
        public CharacterClass Class = CharacterClass.Warrior;
        public Stats BaseStats = new Stats();
        public List<GoonzuGame.Items.Item> Inventory = new List<GoonzuGame.Items.Item>();
        public Vector3 Position = new Vector3(0, 0, 0);

        public Character() {}

        public Character(string name, CharacterClass charClass)
        {
            Name = name;
            Class = charClass;
            InitializeCharacter();
        }

        private void InitializeCharacter()
        {
            MaxHealth = 100 + (BaseStats.Constitution - 10) * 10;
            Health = MaxHealth;
            MaxMana = 50 + (BaseStats.Intelligence - 10) * 5;
            Mana = MaxMana;
        }

        public void Move(Vector3 direction)
        {
            Position += direction;
            Console.WriteLine($"{Name} moved to {Position}");
        }

        public void Attack(Character target)
        {
            int damage = CalculateDamage();
            target.TakeDamage(damage);
            Console.WriteLine($"{Name} attacked {target.Name} for {damage} damage");
        }

        private int CalculateDamage()
        {
            // Simple damage calculation
            return BaseStats.Strength + new Random().Next(1, 7);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Console.WriteLine($"{Name} has died!");
            // Handle death logic: respawn, game over, etc.
        }

        public void PickUpItem(GoonzuGame.Items.Item item)
        {
            Inventory.Add(item);
            Console.WriteLine($"{Name} picked up {item.Name}");
        }

        public void LevelUp()
        {
            Level++;
            Experience = 0;
            ExperienceToNextLevel = Level * 100;
            // Increase stats
            BaseStats.Strength += 1;
            BaseStats.Dexterity += 1;
            BaseStats.Intelligence += 1;
            BaseStats.Wisdom += 1;
            BaseStats.Constitution += 1;
            BaseStats.Charisma += 1;
            MaxHealth += 10;
            Health = MaxHealth;
            MaxMana += 5;
            Mana = MaxMana;
            Console.WriteLine($"{Name} leveled up to {Level}!");
            // Trigger achievement
            GoonzuGame.Achievements.AchievementManager.Instance?.TrackProgress("Level Up", 1);
        }

        public void GainExperience(int exp)
        {
            Experience += exp;
            if (Experience >= ExperienceToNextLevel)
            {
                LevelUp();
            }
        }

        public void UseItem(GoonzuGame.Items.Item item)
        {
            if (Inventory.Contains(item))
            {
                // Apply item effects
                if (item.Type == "Potion")
                {
                    Health = Math.Min(Health + 20, MaxHealth);
                }
                Inventory.Remove(item);
                Console.WriteLine($"{Name} used {item.Name}");
            }
        }
    }
}
