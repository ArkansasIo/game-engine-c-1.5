using System.Collections.Generic;
namespace GoonzuGame.World {
    using GoonzuGame.Items;
    public class WorldManager {
        public List<Item> WorldItems { get; set; }
        public WorldManager() { WorldItems = new List<Item>(); }
        public void Load() { }
    }
}
