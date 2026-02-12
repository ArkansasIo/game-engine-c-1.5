using System;
using System.Collections.Generic;

namespace GoonzuGame.DnD5e
{
    public class SpellDef
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public string School { get; set; }
        public string Description { get; set; }
        public List<string> Classes { get; set; }
        public SpellDef(string name, int level, string school, string description, List<string> classes)
        {
            Name = name;
            Level = level;
            School = school;
            Description = description;
            Classes = classes;
        }
        public void Display()
        {
            Console.WriteLine($"Spell: {Name}, Level: {Level}, School: {School}, Description: {Description}, Classes: {string.Join(", ", Classes)}");
        }
    }
}