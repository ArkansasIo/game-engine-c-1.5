using System;
using System.Collections.Generic;

namespace GoonzuGame.Pets
{
    public class PetDef
    {
        public string Name { get; set; }
        public string Species { get; set; }
        public int Level { get; set; }
        public List<string> Abilities { get; set; }
        public PetDef(string name, string species, int level, List<string> abilities)
        {
            Name = name;
            Species = species;
            Level = level;
            Abilities = abilities;
        }
        public void Display()
        {
            Console.WriteLine($"Pet: {Name}, Species: {Species}, Level: {Level}, Abilities: {string.Join(", ", Abilities)}");
        }
    }
}