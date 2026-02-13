namespace GoonzuGame.DnD5e {
    public class Equipment {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Cost { get; set; }
        public Equipment(string name, string type, int cost) {
            Name = name;
            Type = type;
            Cost = cost;
        }
    }
}
