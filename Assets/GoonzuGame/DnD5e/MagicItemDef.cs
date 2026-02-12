using System;
using System.Collections.Generic;

namespace GoonzuGame.DnD5e
{
    public class MagicItemDef
    {
        public string Name { get; set; }
        public string Rarity { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public List<string> Properties { get; set; }
        public MagicItemDef(string name, string rarity, string type, string description, List<string> properties)
        {
            Name = name;
            Rarity = rarity;
            Type = type;
            Description = description;
            Properties = properties;
        }
        public void Display()
        {
            Console.WriteLine($"Magic Item: {Name}, Rarity: {Rarity}, Type: {Type}, Description: {Description}, Properties: {string.Join(", ", Properties)}");
        }
    }
}