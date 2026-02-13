namespace GoonzuGame.Items {
    public class Armor : Item {
        public int ArmorId { get; set; }
        public int Defense { get; set; }
        public string Rarity { get; set; }
        public Armor(int id, string name, int defense, string rarity) : base(name, "Armor") {
            ArmorId = id;
            Defense = defense;
            Rarity = rarity;
        }
        public void Equip() { }
    }
}
