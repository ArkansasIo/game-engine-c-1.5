namespace GoonzuGame.Characters
{
    public class Character
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }

        public void Move(string direction) { }
        public void LevelUp() { }
        public void PickUpItem(GoonzuGame.Items.Item item) { }
    }
}
