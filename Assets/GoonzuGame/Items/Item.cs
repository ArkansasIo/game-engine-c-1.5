namespace GoonzuGame.Items {
    public class Item {
        public string Name { get; set; }
        public string Type { get; set; }
        public Item() {}
        public Item(string name, string type) { Name = name; Type = type; }
    }
}
