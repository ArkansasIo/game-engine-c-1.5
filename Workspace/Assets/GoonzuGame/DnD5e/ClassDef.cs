using System;
using System.Collections.Generic;

namespace GoonzuGame.DnD5e
{
    public class ClassDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> SubClasses { get; set; }
        public ClassDef(string name, string description, List<string> subClasses)
        {
            Name = name;
            Description = description;
            SubClasses = subClasses;
        }
        public void Display()
        {
            Console.WriteLine($"Class: {Name}, Description: {Description}, SubClasses: {string.Join(", ", SubClasses)}");
        }
    }
}