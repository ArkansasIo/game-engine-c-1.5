namespace GameEngine.Player
{
    public class PlayerPlaceholder
    {
        public string Name { get; set; } = "Player";
        public int Level { get; set; } = 1;
        public void Move(string direction) {
            System.Console.WriteLine($"{Name} moves {direction} (stub).");
        }
        public void Attack() {
            System.Console.WriteLine($"{Name} attacks (stub).");
        }
    }
}
