using System.Collections.Generic;
using System;

namespace GoonzuGame.Skills
{
    public class SkillTreeManager
    {
        public static SkillTreeManager Instance { get; } = new SkillTreeManager();

        public List<Skill> Skills = new List<Skill>();
        public GoonzuGame.Characters.Character? PlayerCharacter;

        private SkillTreeManager()
        {
            InitializeSkills();
        }

        private void InitializeSkills()
        {
            // Add default skills
            Skills.Add(new OffensiveSkill("Fireball", "Throws a fireball", 20, 10, 2f));
            Skills.Add(new DefensiveSkill("Heal", "Heals the target", 30, 15, 5f));
            Skills.Add(new UtilitySkill("Teleport", "Teleports short distance", 20, 10f));
            Skills.Add(new PassiveSkill("Regeneration", "Slowly regenerates health"));
        }

        public void UpgradeSkill(string skillName)
        {
            var skill = Skills.Find(s => s.Name == skillName);
            if (skill != null)
            {
                skill.LevelUp();
            }
        }

        public void ResetSkill(string skillName)
        {
            var skill = Skills.Find(s => s.Name == skillName);
            if (skill != null)
            {
                skill.Level = 1;
                Console.WriteLine($"Reset skill: {skillName}");
            }
        }

        public void UnlockSkill(string skillName)
        {
            var skill = Skills.Find(s => s.Name == skillName);
            if (skill != null && !skill.CanUse(PlayerCharacter)) // Assuming passive are always "usable"
            {
                // For passive, just level up or something
                Console.WriteLine($"Skill '{skillName}' unlocked.");
            }
        }

        public void LevelUpSkill(string skillName)
        {
            UpgradeSkill(skillName);
        }

        public void UseAbility(string abilityName, GoonzuGame.Characters.Character? target = null)
        {
            var skill = Skills.Find(s => s.Name == abilityName);
            if (skill != null)
            {
                skill.Use(PlayerCharacter, target);
            }
        }

        public void TickCooldowns(float deltaTime)
        {
            // Update LastUsedTime
            foreach (var skill in Skills)
            {
                skill.UpdateCooldown(deltaTime);
            }
        }
    }
}
