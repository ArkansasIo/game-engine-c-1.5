using System;
using GoonzuGame.Items;

namespace GoonzuGame.Weapons
{
    public class WeaponDef : Item
    {
        public int Attack { get; set; }
        public int Speed { get; set; }
        public string PvPType { get; set; }
        public string PvEType { get; set; }
        public string Theme { get; set; }
        public WeaponDef(string name, int attack, int speed, string rarity, string pvpType, string pveType, string theme)
            : base(name, rarity)
        {
            Attack = attack;
            Speed = speed;
            PvPType = pvpType;
            PvEType = pveType;
            Theme = theme;
        }
    }
}