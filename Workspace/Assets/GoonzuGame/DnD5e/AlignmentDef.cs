using System;

namespace GoonzuGame.DnD5e
{
    public class AlignmentDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public AlignmentDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Alignment: {Name}, Description: {Description}");
        }
    }
}