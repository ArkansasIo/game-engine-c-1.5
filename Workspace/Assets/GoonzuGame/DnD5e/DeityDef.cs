using System;

namespace GoonzuGame.DnD5e
{
    public class DeityDef
    {
        public string Name { get; set; }
        public string Domain { get; set; }
        public string Description { get; set; }
        public DeityDef(string name, string domain, string description)
        {
            Name = name;
            Domain = domain;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Deity: {Name}, Domain: {Domain}, Description: {Description}");
        }
    }
}