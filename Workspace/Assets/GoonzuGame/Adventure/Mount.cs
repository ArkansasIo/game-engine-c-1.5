namespace GoonzuGame.Adventure {
    public class Mount {
        public int MountId { get; set; }
        public string Name { get; set; }
        public int Speed { get; set; }
        public string Type { get; set; }
        public Mount(int id, string name, int speed, string type) {
            MountId = id;
            Name = name;
            Speed = speed;
            Type = type;
        }
        public void Ride() { }
    }
}
