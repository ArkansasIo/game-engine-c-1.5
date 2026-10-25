using System;
using System.Collections.Generic;

namespace GoonzuGame.DnD5e
{
    public class EquipmentDef
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Rarity { get; set; }
        public string Description { get; set; }
        public List<string> Properties { get; set; }
        public EquipmentDef(string name, string type, string rarity, string description, List<string> properties)
        {
            Name = name;
            Type = type;
            Rarity = rarity;
            Description = description;
            Properties = properties;
        }
        public void Display()
        {
            Console.WriteLine($"Equipment: {Name}, Type: {Type}, Rarity: {Rarity}, Description: {Description}, Properties: {string.Join(", ", Properties)}");
        }
    }
}