using System;

namespace GoonzuGame.GUI
{
    using GoonzuGame.Skills;
    using System.Collections.Generic;

    public class SkillsWindow : UIWindow
    {
        public List<Skill> Skills { get; set; }
        public SkillsWindow()
        {
            Skills = new List<Skill>();
        }
        public override void Show()
        {
            Console.WriteLine("Showing SkillsWindow");
            DisplaySkills();
        }
        public void DisplaySkills()
        {
            Console.WriteLine("Skills:");
            foreach (var skill in Skills)
                Console.WriteLine($"- {skill.Name} (Level {skill.Level})");
        }
        public void AddSkill(Skill skill)
        {
            Skills.Add(skill);
            Console.WriteLine($"Added skill: {skill.Name}");
        }
        public void RemoveSkill(Skill skill)
        {
            Skills.Remove(skill);
            Console.WriteLine($"Removed skill: {skill.Name}");
        }
    }
}
using UnityEngine;
using GoonzuUI.Windows;

namespace GoonzuUI.Windows
{
    public sealed class SkillsWindow : UIWindow
    {
        // Add fields for skill tree UI, skill buttons, etc.
        // Implement skill leveling logic as needed
    }
}
