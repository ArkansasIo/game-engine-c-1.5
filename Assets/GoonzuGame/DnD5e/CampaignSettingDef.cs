using System;

namespace GoonzuGame.DnD5e
{
    public class CampaignSettingDef
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CampaignSettingDef(string name, string description)
        {
            Name = name;
            Description = description;
        }
        public void Display()
        {
            Console.WriteLine($"Campaign Setting: {Name}, Description: {Description}");
        }
    }
}