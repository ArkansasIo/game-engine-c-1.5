namespace GoonzuGame.Equipment {
    public class EquipmentDef : GoonzuGame.Items.Item {
        public string Slot { get; set; }
        public int Power { get; set; }
        public int Durability { get; set; }
        public EquipmentDef(string name, string type, int power, int durability) : base(name, type) {
            Power = power;
            Durability = durability;
        }
    }
}
