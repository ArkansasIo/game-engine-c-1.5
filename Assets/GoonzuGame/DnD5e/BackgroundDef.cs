using System;
using System.Collections.Generic;

namespace GoonzuGame.DnD5e
{
    public class BackgroundDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Tools { get; set; }
        public List<string> Languages { get; set; }
        public List<string> Features { get; set; }
        public BackgroundDef(string name, string description, List<string> skills, List<string> tools, List<string> languages, List<string> features)
        {
            Name = name;
            Description = description;
            Skills = skills;
            Tools = tools;
            Languages = languages;
            Features = features;
        }
        public void Display()
        {
            Console.WriteLine($"Background: {Name}, Description: {Description}, Skills: {string.Join(", ", Skills)}, Tools: {string.Join(", ", Tools)}, Languages: {string.Join(", ", Languages)}, Features: {string.Join(", ", Features)}");
        }
    }
}