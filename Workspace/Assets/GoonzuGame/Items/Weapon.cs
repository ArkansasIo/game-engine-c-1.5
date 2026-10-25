namespace GoonzuGame.Items {
    public class Weapon : Item {
        public int WeaponId { get; set; }
        public int Damage { get; set; }
        public string Rarity { get; set; }
        public Weapon(int id, string name, int damage, string rarity) : base(name, "Weapon") {
            WeaponId = id;
            Damage = damage;
            Rarity = rarity;
        }
        public void Attack() { }
    }
}
