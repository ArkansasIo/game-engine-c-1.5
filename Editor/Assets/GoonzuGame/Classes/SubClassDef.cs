using System;
using System.Collections.Generic;

namespace GoonzuGame.Classes
{
    public class SubClassDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> SubTypes { get; set; }
        public SubClassDef(string name, string description, List<string> subTypes)
        {
            Name = name;
            Description = description;
            SubTypes = subTypes;
        }
        public void Display()
        {
            Console.WriteLine($"SubClass: {Name}, Description: {Description}, SubTypes: {string.Join(", ", SubTypes)}");
        }
    }
}