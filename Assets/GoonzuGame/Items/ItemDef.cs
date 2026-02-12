using System;

namespace GoonzuGame.Items
{
    public class ItemDef
    {
        public string Name { get; set; }
        public string Rarity { get; set; }
        public string Theme { get; set; }
        public string Description { get; set; }
        public ItemDef(string name, string rarity, string theme, string description)
        {
            Name = name;
            Rarity = rarity;
            Theme = theme;
            Description = description;
        }
    }
}