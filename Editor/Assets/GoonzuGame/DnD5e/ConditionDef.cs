using System;

namespace GoonzuGame.DnD5e
{
    public class ConditionDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ConditionDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Condition: {Name}, Description: {Description}");
        }
    }
}