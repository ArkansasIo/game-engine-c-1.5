using System;

namespace GoonzuGame.SaveLoad
{
    using System.IO;
    using System.Text.Json;
    using GoonzuGame.Characters;

    public class SaveLoadManager
        public void AutoSave() {
            System.Console.WriteLine("Auto-saving game...");
        }
        public void AutoLoad() {
            System.Console.WriteLine("Auto-loading game...");
        }
    {
        private string saveDirectory = "./Saves/";
        public SaveLoadManager()
        {
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);
        }

        public void SavePlayerData(Character player, int slot)
        {
            string path = Path.Combine(saveDirectory, $"player_slot_{slot}.json");
            string json = JsonSerializer.Serialize(player);
            File.WriteAllText(path, json);
            Console.WriteLine($"Player data saved to slot {slot}.");
        }

        public Character LoadPlayerData(int slot)
        {
            string path = Path.Combine(saveDirectory, $"player_slot_{slot}.json");
            if (!File.Exists(path))
            {
                Console.WriteLine($"No save found for slot {slot}.");
                return null;
            }
            string json = File.ReadAllText(path);
            Character player = JsonSerializer.Deserialize<Character>(json);
            Console.WriteLine($"Player data loaded from slot {slot}.");
            return player;
        }

        public void Autosave(Character player)
        {
            SavePlayerData(player, 0);
            Console.WriteLine("Autosave complete.");
        }

        public void ShowSaveMenu()
        {
            Console.WriteLine("Save/Load Menu:");
            for (int slot = 0; slot < 3; slot++)
            {
                string path = Path.Combine(saveDirectory, $"player_slot_{slot}.json");
                string status = File.Exists(path) ? "Occupied" : "Empty";
                Console.WriteLine($"Slot {slot}: {status}");
            }
        }
    }
}
