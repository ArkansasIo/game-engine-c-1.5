using System;

namespace GoonzuGame.Skills
{
    using System.Collections.Generic;

    public class Skill
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public bool IsUnlocked { get; set; }
        public int Cooldown { get; set; }
        public Skill(string name)
        {
            Name = name;
            Level = 1;
            IsUnlocked = false;
            Cooldown = 0;
        }
    }

    public class SkillTreeManager
    {
        public List<Skill> Skills { get; set; }

        public SkillTreeManager()
        {
            Skills = new List<Skill>();
        }

        public void UnlockSkill(string skillName)
        {
            var skill = Skills.Find(s => s.Name == skillName);
            if (skill != null && !skill.IsUnlocked)
            {
                skill.IsUnlocked = true;
                Console.WriteLine($"Skill '{skillName}' unlocked.");
            }
        }

        public void LevelUpSkill(string skillName)
        {
            var skill = Skills.Find(s => s.Name == skillName);
            if (skill != null && skill.IsUnlocked)
            {
                skill.Level++;
                Console.WriteLine($"Skill '{skillName}' leveled up to {skill.Level}.");
            }
        }

        public void UseAbility(string abilityName)
        {
            var skill = Skills.Find(s => s.Name == abilityName);
            if (skill != null && skill.IsUnlocked && skill.Cooldown == 0)
            {
                Console.WriteLine($"Ability '{abilityName}' used.");
                skill.Cooldown = 3; // Example cooldown
            }
            else
            {
                Console.WriteLine($"Ability '{abilityName}' is not ready.");
            }
        }

        public void TickCooldowns()
        {
            foreach (var skill in Skills)
            {
                if (skill.Cooldown > 0)
                    skill.Cooldown--;
            }
        }
    }
}
