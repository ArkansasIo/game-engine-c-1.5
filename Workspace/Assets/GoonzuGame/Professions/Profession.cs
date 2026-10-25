using System.Collections.Generic;
using System;

namespace GoonzuGame.Professions
{
    [System.Serializable]
    public class Profession
    {
        public string Name;
        public string Description;
        public int Level;
        public int Experience;
        public int ExperienceToNextLevel;
        public List<string> Skills = new List<string>(); // Skill names
        public List<string> Recipes = new List<string>(); // Recipe names

        public Profession(string name, string description)
        {
            Name = name;
            Description = description;
            Level = 1;
            Experience = 0;
            ExperienceToNextLevel = 100;
        }

        public void GainExperience(int amount)
        {
            Experience += amount;
            if (Experience >= ExperienceToNextLevel)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
            Experience = 0;
            ExperienceToNextLevel = Level * 100;
            Console.WriteLine($"{Name} profession leveled up to {Level}!");
            // Unlock new skills/recipes
        }

        public void Work()
        {
            // Simulate work
            GainExperience(10);
            Console.WriteLine($"Working as {Name}, gained 10 experience.");
            // Perhaps produce items or earn gold
            // GoonzuGame.Trade.TradeManager.Instance.AddGold(5); // Placeholder
        }

        public bool HasSkill(string skillName)
        {
            return Skills.Contains(skillName);
        }

        public bool HasRecipe(string recipeName)
        {
            return Recipes.Contains(recipeName);
        }
    }

    public class ProfessionManager
    {
        public static ProfessionManager Instance { get; } = new ProfessionManager();

        public List<Profession> AvailableProfessions = new List<Profession>();
        public Profession CurrentProfession;

        private ProfessionManager()
        {
            InitializeProfessions();
        }

        private void InitializeProfessions()
        {
            AvailableProfessions.Add(new Profession("Blacksmith", "Forge weapons and armor"));
            AvailableProfessions.Add(new Profession("Alchemist", "Brew potions and elixirs"));
            AvailableProfessions.Add(new Profession("Tailor", "Create clothing and accessories"));
        }

        public void SetProfession(Profession profession)
        {
            CurrentProfession = profession;
            Console.WriteLine($"Set profession to {profession.Name}");
        }

        public void Work()
        {
            if (CurrentProfession != null)
            {
                CurrentProfession.Work();
            }
        }

        public List<Profession> GetAvailableProfessions()
        {
            return AvailableProfessions;
        }
    }
}
