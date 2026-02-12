using System;
using System.Collections.Generic;

namespace GoonzuGame.DnD5e
{
    public class LevelProgressionDef
    {
        public int Level { get; set; }
        public List<string> Features { get; set; }
        public LevelProgressionDef(int level, List<string> features)
        {
            Level = level;
            Features = features;
        }
        public void Display()
        {
            Console.WriteLine($"Level: {Level}, Features: {string.Join(", ", Features)}");
        }
    }
}