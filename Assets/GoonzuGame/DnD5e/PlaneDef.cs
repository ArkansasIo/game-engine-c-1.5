using System;

namespace GoonzuGame.DnD5e
{
    public class PlaneDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public PlaneDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Plane: {Name}, Description: {Description}");
        }
    }
}