using System;
using System.Collections.Generic;

namespace GoonzuGame.DnD5e
{
    public class OptionalSystemDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Features { get; set; }
        public OptionalSystemDef(string name, string description, List<string> features)
        {
            Name = name;
            Description = description;
            Features = features;
        }
        public void Display()
        {
            Console.WriteLine($"Optional System: {Name}, Description: {Description}, Features: {string.Join(", ", Features)}");
        }
    }
}