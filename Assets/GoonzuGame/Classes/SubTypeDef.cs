using System;

namespace GoonzuGame.Classes
{
    public class SubTypeDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public SubTypeDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"SubType: {Name}, Description: {Description}");
        }
    }
}