using System;

namespace GoonzuGame.Trials
{
    public class TrialDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TrialDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Trial: {Name}, Description: {Description}");
        }
    }
}