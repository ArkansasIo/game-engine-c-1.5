namespace GoonzuGame.Monsters {
    public class Monster {
        public int MonsterId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public Monster() {}
        public Monster(int id, string name, int level, int health) {
            MonsterId = id;
            Name = name;
            Level = level;
            Health = health;
        }
        public void Attack() { }
        public void DropLoot() { }
    }
}
