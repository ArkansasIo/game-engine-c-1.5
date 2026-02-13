namespace GoonzuGame.Monsters {
    public class Boss : Monster {
        public int BossId { get; set; }
        public string SpecialAbility { get; set; }
        public Boss(int id, string name, int level, string ability) : base() {
            BossId = id;
            Name = name;
            Level = level;
            SpecialAbility = ability;
        }
        public void UseSpecial() { }
    }
}
