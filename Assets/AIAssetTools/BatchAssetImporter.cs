using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace AIAssetTools
{
    /// <summary>
    /// Batch import tool for AI-generated assets
    /// </summary>
    public class BatchAssetImporter : EditorWindow
    {
        [MenuItem("Tools/AI Asset Tools/Batch Importer")]
        static void ShowWindow()
        {
            GetWindow<BatchAssetImporter>("Batch Asset Importer");
        }

        private string importPath = "Assets/GoonzuGame";
        private bool autoSetupSprites = true;
        private bool createSpriteAtlases = false;
        private Vector2 scrollPos;

        void OnGUI()
        {
            GUILayout.Label("Batch Asset Importer", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Import path
            EditorGUILayout.LabelField("Import Path:");
            importPath = EditorGUILayout.TextField(importPath);

            // Options
            autoSetupSprites = EditorGUILayout.Toggle("Auto Setup Sprites", autoSetupSprites);
            createSpriteAtlases = EditorGUILayout.Toggle("Create Sprite Atlases", createSpriteAtlases);

            GUILayout.Space(10);

            if (GUILayout.Button("Import All PNGs in Folder"))
            {
                ImportAllPNGs();
            }

            if (GUILayout.Button("Setup Existing Assets"))
            {
                SetupExistingAssets();
            }

            if (GUILayout.Button("Create Sprite Atlases"))
            {
                if (createSpriteAtlases)
                    CreateAtlases();
            }

            if (GUILayout.Button("Validate Assets"))
            {
                ValidateAssets();
            }

            GUILayout.Space(10);
            GUILayout.Label("Quick Actions:", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            if (GUILayout.Button("Import Characters"))
            {
                ImportCategory("Characters");
            }

            if (GUILayout.Button("Import Buildings"))
            {
                ImportCategory("Buildings");
            }

            if (GUILayout.Button("Import Tiles"))
            {
                ImportCategory("Tiles");
            }

            if (GUILayout.Button("Import Items"))
            {
                ImportCategory("Items");
            }

            if (GUILayout.Button("Import UI"))
            {
                ImportCategory("UI");
            }

            EditorGUILayout.EndScrollView();
        }

        void ImportAllPNGs()
        {
            string[] pngFiles = Directory.GetFiles(importPath, "*.png", SearchOption.AllDirectories);

            foreach (string pngFile in pngFiles)
            {
                // Force reimport
                AssetDatabase.ImportAsset(pngFile, ImportAssetOptions.ForceUpdate);
            }

            AssetDatabase.Refresh();
            Debug.Log($"Imported {pngFiles.Length} PNG files");
        }

        void SetupExistingAssets()
        {
            string[] pngFiles = Directory.GetFiles(importPath, "*.png", SearchOption.AllDirectories);

            foreach (string pngFile in pngFiles)
            {
                TextureImporter importer = AssetImporter.GetAtPath(pngFile) as TextureImporter;
                if (importer != null)
                {
                    // Force reimport with current settings
                    AssetDatabase.ImportAsset(pngFile, ImportAssetOptions.ForceUpdate);
                }
            }

            Debug.Log($"Setup {pngFiles.Length} existing assets");
        }

        void ImportCategory(string category)
        {
            string categoryPath = Path.Combine(importPath, category);
            if (!Directory.Exists(categoryPath))
            {
                Debug.LogWarning($"Category folder not found: {categoryPath}");
                return;
            }

            string[] pngFiles = Directory.GetFiles(categoryPath, "*.png", SearchOption.AllDirectories);

            foreach (string pngFile in pngFiles)
            {
                AssetDatabase.ImportAsset(pngFile, ImportAssetOptions.ForceUpdate);
            }

            AssetDatabase.Refresh();
            Debug.Log($"Imported {pngFiles.Length} {category} assets");
        }

        void CreateAtlases()
        {
            // Create sprite atlases for performance
            string atlasPath = "Assets/GoonzuGame/Spritesheets";

            if (!Directory.Exists(atlasPath))
                Directory.CreateDirectory(atlasPath);

            // Create UI atlas
            CreateAtlasForCategory("UI", atlasPath);
            // Create character atlas
            CreateAtlasForCategory("Characters", atlasPath);
            // Create item atlas
            CreateAtlasForCategory("Items", atlasPath);

            AssetDatabase.Refresh();
            Debug.Log("Created sprite atlases");
        }

        void CreateAtlasForCategory(string category, string atlasPath)
        {
            string categoryPath = Path.Combine(importPath, category);
            if (!Directory.Exists(categoryPath)) return;

            string[] pngFiles = Directory.GetFiles(categoryPath, "*.png", SearchOption.AllDirectories);

            if (pngFiles.Length == 0) return;

            // Create a new sprite atlas asset
            string atlasAssetPath = $"{atlasPath}/{category}Atlas.spriteatlas";
            SpriteAtlas spriteAtlas = new SpriteAtlas();

            // Add sprites to atlas
            List<Sprite> sprites = new List<Sprite>();
            foreach (string pngFile in pngFiles)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngFile);
                if (sprite != null)
                    sprites.Add(sprite);
            }

            // Note: In Unity 2020+, you'd use SpriteAtlasExtensions.Add
            // For now, we'll just log
            Debug.Log($"Would create atlas {atlasAssetPath} with {sprites.Count} sprites");

            AssetDatabase.CreateAsset(spriteAtlas, atlasAssetPath);
        }

        void ValidateAssets()
        {
            string[] pngFiles = Directory.GetFiles(importPath, "*.png", SearchOption.AllDirectories);
            int validAssets = 0;
            int invalidAssets = 0;

            foreach (string pngFile in pngFiles)
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(pngFile);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngFile);

                if (texture != null && sprite != null)
                {
                    validAssets++;
                }
                else
                {
                    invalidAssets++;
                    Debug.LogWarning($"Invalid asset: {pngFile}");
                }
            }

            Debug.Log($"Validation complete: {validAssets} valid, {invalidAssets} invalid assets");
        }
    }
}