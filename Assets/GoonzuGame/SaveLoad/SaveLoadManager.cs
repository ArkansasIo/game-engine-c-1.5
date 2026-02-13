using System.IO;
using System.Text.Json;

namespace GoonzuGame.SaveLoad
{
    [System.Serializable]
    public class Vector3Data
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3Data(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator GoonzuGame.Characters.Vector3(Vector3Data data)
        {
            return new GoonzuGame.Characters.Vector3(data.X, data.Y, data.Z);
        }

        public static implicit operator Vector3Data(GoonzuGame.Characters.Vector3 vec)
        {
            return new Vector3Data(vec.X, vec.Y, vec.Z);
        }
    }

    [System.Serializable]
    public class SaveData
    {
        public string? PlayerName { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        public GoonzuGame.Characters.CharacterClass Class { get; set; }
        public GoonzuGame.Characters.Stats? BaseStats { get; set; }
        public Vector3Data? Position { get; set; }
        // Add more fields as needed
    }

    public class SaveLoadManager
    {
        public static SaveLoadManager Instance { get; } = new SaveLoadManager();

        private string saveDirectory = "Saves";

        private SaveLoadManager()
        {
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
        }

        public void AutoSave(GoonzuGame.Characters.Character player)
        {
            SavePlayerData(player, 0);
            Console.WriteLine("Auto-saving game...");
        }

        public void AutoLoad(GoonzuGame.Characters.Character player)
        {
            LoadPlayerData(0, player);
            Console.WriteLine("Auto-loading game...");
        }

        public void SavePlayerData(GoonzuGame.Characters.Character player, int slot)
        {
            if (player == null) return;

            SaveData data = new SaveData
            {
                PlayerName = player.Name,
                Level = player.Level,
                Experience = player.Experience,
                Health = player.Health,
                Mana = player.Mana,
                Class = player.Class,
                BaseStats = player.BaseStats,
                Position = player.Position
            };

            string json = JsonSerializer.Serialize(data);
            string filePath = Path.Combine(saveDirectory, $"SaveSlot_{slot}.json");
            File.WriteAllText(filePath, json);
            Console.WriteLine($"Player data saved to slot {slot}.");
        }

        public void LoadPlayerData(int slot, GoonzuGame.Characters.Character player)
        {
            string filePath = Path.Combine(saveDirectory, $"SaveSlot_{slot}.json");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"No save found for slot {slot}.");
                return;
            }

            string json = File.ReadAllText(filePath);
            SaveData? data = JsonSerializer.Deserialize<SaveData>(json);
            if (data == null)
            {
                Console.WriteLine("Failed to deserialize save data.");
                return;
            }
            if (player != null && data.PlayerName != null && data.BaseStats != null && data.Position != null)
            {
                player.Name = data.PlayerName;
                player.Level = data.Level;
                player.Experience = data.Experience;
                player.Health = data.Health;
                player.Mana = data.Mana;
                player.Class = data.Class;
                player.BaseStats = data.BaseStats;
                player.Position = data.Position;
                Console.WriteLine($"Player data loaded from slot {slot}.");
            }
        }

        public void Autosave(GoonzuGame.Characters.Character player)
        {
            SavePlayerData(player, 0);
            Console.WriteLine("Autosave complete.");
        }

        public void ShowSaveMenu()
        {
            Console.WriteLine("Save/Load Menu:");
            for (int slot = 0; slot < 3; slot++)
            {
                string filePath = Path.Combine(saveDirectory, $"SaveSlot_{slot}.json");
                string status = File.Exists(filePath) ? "Occupied" : "Empty";
                Console.WriteLine($"Slot {slot}: {status}");
            }
        }

        // For compatibility
        public void SaveGame(object player)
        {
            if (player is GoonzuGame.Characters.Character character)
            {
                SavePlayerData(character, 0);
            }
            Console.WriteLine("Game saved.");
        }

        public void LoadGame(string saveName, GoonzuGame.Characters.Character player)
        {
            // Assume slot 0 for simplicity
            LoadPlayerData(0, player);
            Console.WriteLine($"Game loaded: {saveName}");
        }
    }
}
