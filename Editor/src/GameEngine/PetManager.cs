namespace GameEngine
{
    /// <summary>
    /// Manages player pets and pet-related features.
    /// </summary>
    public class PetManager
    {
        public void AdoptPet(string player, string petName)
        {
            System.Console.WriteLine($"{player} adopted pet: {petName}");
        }
        public void ReleasePet(string player, string petName)
        {
            System.Console.WriteLine($"{player} released pet: {petName}");
        }
        public void ListPets(string player)
        {
            System.Console.WriteLine($"Listing pets for {player} (stub).");
        }
    }
}
