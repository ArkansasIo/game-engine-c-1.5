using System;
using System.Collections.Generic;

namespace GoonzuGame.DnD5e
{
    public class SubClassDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Features { get; set; }
        public SubClassDef(string name, string description, List<string> features)
        {
            Name = name;
            Description = description;
            Features = features;
        }
        public void Display()
        {
            Console.WriteLine($"SubClass: {Name}, Description: {Description}, Features: {string.Join(", ", Features)}");
        }
    }
}