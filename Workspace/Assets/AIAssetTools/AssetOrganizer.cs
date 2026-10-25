using UnityEngine;
using UnityEditor;
using System.IO;

namespace AIAssetTools
{
    /// <summary>
    /// Utility class for organizing AI-generated assets
    /// </summary>
    public static class AssetOrganizer
    {
        public static void CreateAssetFolders()
        {
            string basePath = "Assets/GoonzuGame/";
            
            string[] folders = {
                "Characters",
                "Buildings", 
                "Tiles",
                "Items/Weapons",
                "Items/Armor",
                "Items/Materials",
                "Items/Consumables",
                "Creatures/Mounts",
                "Creatures/Monsters",
                "UI/Panels",
                "UI/Icons",
                "UI/Bars",
                "UI/Windows",
                "Spritesheets"
            };
            
            foreach (string folder in folders)
            {
                string fullPath = Path.Combine(basePath, folder);
                if (!AssetDatabase.IsValidFolder(fullPath))
                {
                    AssetDatabase.CreateFolder(Path.GetDirectoryName(fullPath), Path.GetFileName(fullPath));
                    Debug.Log($"Created folder: {fullPath}");
                }
            }
            
            AssetDatabase.Refresh();
        }
        
        public static void MoveAssetToCategory(string assetPath, string category)
        {
            string targetPath = $"Assets/GoonzuGame/{category}/{Path.GetFileName(assetPath)}";
            AssetDatabase.MoveAsset(assetPath, targetPath);
            Debug.Log($"Moved asset to: {targetPath}");
        }
        
        public static void RenameAssetWithConvention(string assetPath, string prefix)
        {
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            string extension = Path.GetExtension(assetPath);
            string newName = $"{prefix}_{fileName}{extension}";
            string directory = Path.GetDirectoryName(assetPath);
            string newPath = Path.Combine(directory, newName);
            
            AssetDatabase.RenameAsset(assetPath, newName);
            Debug.Log($"Renamed asset to: {newName}");
        }
    }
}