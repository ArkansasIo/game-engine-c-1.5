using System;
using System.Collections.Generic;

namespace GoonzuGame.Professions
{
    public class ProfessionDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Skills { get; set; }
        public string Proficiency { get; set; }
        public ProfessionDef(string name, string description, List<string> skills, string proficiency = "")
        {
            Name = name;
            Description = description;
            Skills = skills;
            Proficiency = proficiency;
        }

        public bool SkillCheck(string skill)
        {
            // DnD: skill check succeeds if skill is in profession's skills
            return Skills.Contains(skill);
        }

        public bool HasProficiency(string prof)
        {
            return Proficiency == prof;
        }

        public void Display()
        {
            Console.WriteLine($"Profession: {Name}, Description: {Description}, Skills: {string.Join(", ", Skills)}, Proficiency: {Proficiency}");
        }
    }
}