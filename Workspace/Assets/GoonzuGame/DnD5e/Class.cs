namespace GoonzuGame.DnD5e {
    public class Class {
        public string Name { get; set; }
        public string HitDie { get; set; }
        public string PrimaryAbility { get; set; }
        public Class(string name, string hitDie, string primaryAbility) {
            Name = name;
            HitDie = hitDie;
            PrimaryAbility = primaryAbility;
        }
    }
}
