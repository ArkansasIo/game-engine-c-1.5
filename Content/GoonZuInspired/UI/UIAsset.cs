using UnityEngine;

namespace GoonZuInspired.UI
{
    // Placeholder for UI window, icon, and font assets
    public class UIAsset : ScriptableObject
    {
        // Linked entities (e.g., zones, professions, etc.)
        private readonly List<string> linkedEntities = new List<string>();

        public string Name { get; set; }
        public Texture2D[] UITextures { get; set; }
        public Font UIFont { get; set; }
        public string Category { get; set; }
        public int Level { get; set; }
        public string Rarity { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        // Add other relevant properties as needed

            public UIAsset(string name, string category, int level, string rarity, string filePath, string description = "") {
                Name = name;
                Category = category;
                Level = level;
                Rarity = rarity;
                FilePath = filePath;
                Description = description;
            }

            // Example: Display asset info
            public void DisplayInfo() {
                Console.WriteLine($"Name: {Name}\nCategory: {Category}\nLevel: {Level}\nRarity: {Rarity}\nFile: {FilePath}\nDescription: {Description}");
            }

            // Asset linking stub
            public void LinkTo(string relatedEntity) {
                if (!string.IsNullOrEmpty(relatedEntity) && !linkedEntities.Contains(relatedEntity)) {
                    linkedEntities.Add(relatedEntity);
                }
            }

            // Get all linked entities
            public IReadOnlyList<string> GetLinkedEntities() {
                return linkedEntities.AsReadOnly();
            }

            // Import/export stub
            public static UIAsset ImportFromFile(string filePath) {
                try {
                    string json = System.IO.File.ReadAllText(filePath);
                    var assetData = System.Text.Json.JsonSerializer.Deserialize<UIAssetData>(json);
                    if (assetData == null) return null;
                    return new UIAsset(
                        assetData.Name,
                        assetData.Category,
                        assetData.Level,
                        assetData.Rarity,
                        assetData.FilePath,
                        assetData.Description
                    );
                } catch (Exception ex) {
                    Console.WriteLine($"Import failed: {ex.Message}");
                    return null;
                }
            }

            // Helper class for JSON (matches export structure)
            private class UIAssetData {
                public string Name { get; set; }
                public string Category { get; set; }
                public int Level { get; set; }
                public string Rarity { get; set; }
                public string FilePath { get; set; }
                public string Description { get; set; }
            }
            public void ExportToFile(string filePath) {
                try {
                    var assetData = new {
                        Name,
                        Category,
                        Level,
                        Rarity,
                        FilePath,
                        Description
                    };
                    string json = System.Text.Json.JsonSerializer.Serialize(assetData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                    System.IO.File.WriteAllText(filePath, json);
                } catch (Exception ex) {
                    ReportError($"Export failed: {ex.Message}");
                }
            }

            // Validation stub
            public bool Validate() {
                // Validation: Name, Category, FilePath must not be empty
                if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Category) || string.IsNullOrEmpty(FilePath))
                    return false;
                // Optionally: Check for valid level and rarity
                if (Level < 0)
                    return false;
                if (string.IsNullOrEmpty(Rarity))
                    return false;
                // Uniqueness check would require a manager context
                return true;
            }

            // Error handling stub
            public void ReportError(string message) {
                // TODO: Implement error reporting
                Console.WriteLine($"Error: {message}");
            }
        }
}
