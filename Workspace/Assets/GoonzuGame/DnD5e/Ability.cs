namespace GoonzuGame.DnD5e {
    public class Ability {
        public string Name { get; set; }
        public int Score { get; set; }
        public int Modifier => (Score - 10) / 2;
        public Ability(string name, int score) {
            Name = name;
            Score = score;
        }
    }
}
