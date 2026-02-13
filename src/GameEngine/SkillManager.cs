namespace GameEngine
{
    /// <summary>
    /// Manages skills, skill learning, and skill usage.
    /// </summary>
    public class SkillManager
    {
        public void LearnSkill(string playerName, string skillName)
        {
            System.Console.WriteLine($"{playerName} learned skill '{skillName}'.");
        }
        public void UseSkill(string playerName, string skillName)
        {
            System.Console.WriteLine($"{playerName} used skill '{skillName}'.");
        }
        public void ListSkills(string playerName)
        {
            System.Console.WriteLine($"Listing skills for {playerName} (stub).");
        }
    }
}
