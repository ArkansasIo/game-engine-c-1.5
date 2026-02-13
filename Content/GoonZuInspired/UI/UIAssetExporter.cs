using System;
using System.IO;
using System.Text.Json;

namespace GoonZuInspired.UI {
    public class UIAssetExporter {
        public static void Export(UIAsset asset, string filePath) {
            try {
                var assetData = new {
                    asset.Name,
                    asset.Category,
                    asset.Level,
                    asset.Rarity,
                    asset.FilePath,
                    asset.Description
                };
                string json = JsonSerializer.Serialize(assetData, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            } catch (Exception ex) {
                Console.WriteLine($"Export failed: {ex.Message}");
            }
        }
    }
}
