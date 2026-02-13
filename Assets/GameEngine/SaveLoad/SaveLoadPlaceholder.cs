using System;
using System.IO;
namespace GameEngine.SaveLoad
{
    public class SaveLoadManager
    {
        public void SaveGame(string filePath, string data)
        {
            File.WriteAllText(filePath, data);
            Console.WriteLine($"Game saved to {filePath}");
        }
        public string LoadGame(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Save file not found: {filePath}");
                return null;
            }
            var data = File.ReadAllText(filePath);
            Console.WriteLine($"Game loaded from {filePath}");
            return data;
        }
    }
}
