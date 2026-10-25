using System;

namespace GoonzuGame.Professions
{
    public class SkillDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public SkillDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Skill: {Name}, Description: {Description}");
        }
    }
}