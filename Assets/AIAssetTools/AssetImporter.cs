using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace AIAssetTools
{
    /// <summary>
    /// Handles importing and setting up AI-generated assets in Unity
    /// </summary>
    public class AssetImporter : AssetPostprocessor
    {
        private static readonly string[] assetFolders = {
            "Assets/GoonzuGame/Characters",
            "Assets/GoonzuGame/Buildings",
            "Assets/GoonzuGame/Tiles",
            "Assets/GoonzuGame/Items",
            "Assets/GoonzuGame/Creatures",
            "Assets/GoonzuGame/UI"
        };

        void OnPreprocessTexture()
        {
            // Check if this is a GoonzuGame asset
            bool isGoonzuAsset = false;
            foreach (string folder in assetFolders)
            {
                if (assetPath.StartsWith(folder))
                {
                    isGoonzuAsset = true;
                    break;
                }
            }

            if (!isGoonzuAsset) return;

            TextureImporter textureImporter = (TextureImporter)assetImporter;

            // Set up import settings based on asset type
            if (assetPath.Contains("/Characters/") || assetPath.Contains("/Creatures/"))
            {
                // Character sprites - pixel perfect
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.filterMode = FilterMode.Point;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.spritePixelsPerUnit = 64;
                textureImporter.mipmapEnabled = false;
            }
            else if (assetPath.Contains("/Tiles/"))
            {
                // Tile textures - seamless
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.filterMode = FilterMode.Point;
                textureImporter.wrapMode = TextureWrapMode.Repeat;
                textureImporter.spritePixelsPerUnit = 64;
                textureImporter.mipmapEnabled = false;
            }
            else if (assetPath.Contains("/UI/"))
            {
                // UI elements
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.filterMode = FilterMode.Bilinear;
                textureImporter.spritePixelsPerUnit = 100;
                textureImporter.mipmapEnabled = false;
            }
            else
            {
                // Default settings for other assets
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.filterMode = FilterMode.Bilinear;
                textureImporter.spritePixelsPerUnit = 64;
                textureImporter.mipmapEnabled = false;
            }

            // Remove placeholder file if it exists
            string placeholderPath = Path.ChangeExtension(assetPath, ".txt");
            if (File.Exists(placeholderPath))
            {
                File.Delete(placeholderPath);
                Debug.Log($"Removed placeholder: {placeholderPath}");
            }
        }

        void OnPostprocessTexture(Texture2D texture)
        {
            // Check if this is a GoonzuGame asset
            bool isGoonzuAsset = false;
            foreach (string folder in assetFolders)
            {
                if (assetPath.StartsWith(folder))
                {
                    isGoonzuAsset = true;
                    break;
                }
            }

            if (!isGoonzuAsset) return;

            Debug.Log($"Imported GoonZu asset: {assetPath}");

            // Auto-create sprite if it's a character or creature
            if (assetPath.Contains("/Characters/") || assetPath.Contains("/Creatures/"))
            {
                CreateSpriteFromTexture(assetPath);
            }
        }

        static void CreateSpriteFromTexture(string texturePath)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(texturePath);
            if (sprite == null)
            {
                Debug.LogWarning($"Could not create sprite from: {texturePath}");
                return;
            }

            // You can add additional sprite setup here if needed
            Debug.Log($"Created sprite: {sprite.name}");
        }
    }
}