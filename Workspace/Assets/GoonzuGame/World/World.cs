using System.Collections.Generic;
using GoonzuGame.Items;

namespace GoonzuGame.World
{
    public class World
    {
        public List<Item> WorldItems { get; set; } = new List<Item>();
        public void Load() { }
    }
}
