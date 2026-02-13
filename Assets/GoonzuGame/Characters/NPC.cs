namespace GoonzuGame.Characters {
    public class NPC : Character {
        public int NpcId { get; set; }
        public string Role { get; set; }
        public NPC(int id, string name, string role) {
            NpcId = id;
            Name = name;
            Role = role;
        }
        public void Interact() { }
    }
}
