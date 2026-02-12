using System;

namespace GoonzuGame.Cities
{
    public class CityDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CityDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"City: {Name}, Description: {Description}");
        }
    }
}