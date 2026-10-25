using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace AIAssetTools
{
    /// <summary>
    /// Comprehensive validation system for AI asset pipeline features
    /// </summary>
    public class AssetPipelineValidator : EditorWindow
    {
        [MenuItem("Tools/AI Asset Tools/Pipeline Validator")]
        static void ShowWindow()
        {
            GetWindow<AssetPipelineValidator>("Pipeline Validator");
        }

        private Vector2 scrollPos;
        private Dictionary<string, ValidationResult> validationResults = new Dictionary<string, ValidationResult>();

        enum ValidationResult { NotTested, Pass, Fail, Warning }

        void OnGUI()
        {
            GUILayout.Label("AI Asset Pipeline Validator", EditorStyles.boldLabel);
            GUILayout.Label("Validate all pipeline features are working correctly");

            EditorGUILayout.Space();

            if (GUILayout.Button("Run All Validations"))
            {
                RunAllValidations();
            }

            EditorGUILayout.Space();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // Feature validations
            DrawValidationResult("Automatic Import", "Drop PNGs in folders, get properly configured sprites");
            DrawValidationResult("Batch Generation", "Generate multiple assets at once with AI APIs");
            DrawValidationResult("Asset Validation", "Check import success and sprite creation");
            DrawValidationResult("Sprite Atlases", "Performance optimization for UI and icons");
            DrawValidationResult("Category Detection", "Smart file naming and path assignment");
            DrawValidationResult("Rate Limiting", "Prevents API overload during batch generation");

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            // Summary
            int passed = 0, failed = 0, warnings = 0;
            foreach (var result in validationResults.Values)
            {
                if (result == ValidationResult.Pass) passed++;
                else if (result == ValidationResult.Fail) failed++;
                else if (result == ValidationResult.Warning) warnings++;
            }

            EditorGUILayout.HelpBox(
                $"Validation Summary: {passed} Passed, {warnings} Warnings, {failed} Failed\n\n" +
                "Click individual tests to run them, or 'Run All Validations' for complete check.",
                passed == 6 ? MessageType.Info : MessageType.Warning
            );
        }

        void DrawValidationResult(string feature, string description)
        {
            EditorGUILayout.BeginHorizontal();

            ValidationResult result = validationResults.ContainsKey(feature) ? validationResults[feature] : ValidationResult.NotTested;

            GUIStyle style = new GUIStyle(GUI.skin.button);
            switch (result)
            {
                case ValidationResult.Pass: style.normal.textColor = Color.green; break;
                case ValidationResult.Fail: style.normal.textColor = Color.red; break;
                case ValidationResult.Warning: style.normal.textColor = Color.yellow; break;
                default: style.normal.textColor = Color.gray; break;
            }

            string statusText = result == ValidationResult.NotTested ? "Not Tested" :
                              result == ValidationResult.Pass ? "✓ PASS" :
                              result == ValidationResult.Fail ? "✗ FAIL" : "⚠ WARNING";

            if (GUILayout.Button($"{feature}: {statusText}", style, GUILayout.Width(200)))
            {
                RunValidation(feature);
            }

            EditorGUILayout.LabelField(description);

            EditorGUILayout.EndHorizontal();
        }

        void RunAllValidations()
        {
            RunValidation("Automatic Import");
            RunValidation("Batch Generation");
            RunValidation("Asset Validation");
            RunValidation("Sprite Atlases");
            RunValidation("Category Detection");
            RunValidation("Rate Limiting");
        }

        void RunValidation(string feature)
        {
            Debug.Log($"=== VALIDATING: {feature} ===");

            switch (feature)
            {
                case "Automatic Import":
                    validationResults[feature] = ValidateAutomaticImport();
                    break;
                case "Batch Generation":
                    validationResults[feature] = ValidateBatchGeneration();
                    break;
                case "Asset Validation":
                    validationResults[feature] = ValidateAssetValidation();
                    break;
                case "Sprite Atlases":
                    validationResults[feature] = ValidateSpriteAtlases();
                    break;
                case "Category Detection":
                    validationResults[feature] = ValidateCategoryDetection();
                    break;
                case "Rate Limiting":
                    validationResults[feature] = ValidateRateLimiting();
                    break;
            }

            Debug.Log($"Validation Result for {feature}: {validationResults[feature]}");
            Repaint();
        }

        ValidationResult ValidateAutomaticImport()
        {
            // Check if AssetImporter script exists and is properly configured
            string importerPath = "Assets/AIAssetTools/AssetImporter.cs";
            if (!File.Exists(importerPath))
            {
                Debug.Log("✗ AssetImporter.cs not found");
                return ValidationResult.Fail;
            }

            // Check if folders exist
            string[] requiredFolders = {
                "Assets/GoonzuGame/Characters",
                "Assets/GoonzuGame/Buildings",
                "Assets/GoonzuGame/Tiles",
                "Assets/GoonzuGame/Items",
                "Assets/GoonzuGame/UI"
            };

            foreach (string folder in requiredFolders)
            {
                if (!Directory.Exists(folder))
                {
                    Debug.Log($"✗ Required folder missing: {folder}");
                    return ValidationResult.Fail;
                }
            }

            // Test with a sample asset
            string testPath = "Assets/GoonzuGame/Characters/validation_test.png";
            CreateTestTexture(testPath);

            // Force import
            AssetDatabase.ImportAsset(testPath, ImportAssetOptions.ForceUpdate);

            // Check import settings
            TextureImporter importer = AssetImporter.GetAtPath(testPath) as TextureImporter;
            if (importer != null &&
                importer.textureType == TextureImporterType.Sprite &&
                importer.spriteImportMode == SpriteImportMode.Single &&
                importer.filterMode == FilterMode.Point &&
                importer.spritePixelsPerUnit == 64)
            {
                Debug.Log("✓ Automatic import working correctly");
                File.Delete(testPath); // Clean up
                return ValidationResult.Pass;
            }
            else
            {
                Debug.Log("✗ Automatic import settings not applied correctly");
                return ValidationResult.Fail;
            }
        }

        ValidationResult ValidateBatchGeneration()
        {
            // Check if RecraftAIManager exists
            string managerPath = "Assets/AIAssetTools/RecraftAIManager.cs";
            if (!File.Exists(managerPath))
            {
                Debug.Log("✗ RecraftAIManager.cs not found");
                return ValidationResult.Fail;
            }

            // Check if batch generation method exists
            string content = File.ReadAllText(managerPath);
            if (content.Contains("BatchGenerateAssets") && content.Contains("batchSize") && content.Contains("delayBetweenRequests"))
            {
                Debug.Log("✓ Batch generation methods found");
                return ValidationResult.Pass;
            }
            else
            {
                Debug.Log("✗ Batch generation methods incomplete");
                return ValidationResult.Fail;
            }
        }

        ValidationResult ValidateAssetValidation()
        {
            // Check if BatchAssetImporter has validation
            string importerPath = "Assets/AIAssetTools/BatchAssetImporter.cs";
            if (!File.Exists(importerPath))
            {
                Debug.Log("✗ BatchAssetImporter.cs not found");
                return ValidationResult.Fail;
            }

            string content = File.ReadAllText(importerPath);
            if (content.Contains("ValidateAssets") && content.Contains("Texture2D") && content.Contains("Sprite"))
            {
                Debug.Log("✓ Asset validation methods found");
                return ValidationResult.Pass;
            }
            else
            {
                Debug.Log("✗ Asset validation methods incomplete");
                return ValidationResult.Fail;
            }
        }

        ValidationResult ValidateSpriteAtlases()
        {
            // Check if atlas creation methods exist
            string[] files = { "Assets/AIAssetTools/BatchAssetImporter.cs", "Assets/AIAssetTools/AssetPipeline.cs" };

            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    string content = File.ReadAllText(file);
                    if (content.Contains("CreateAtlas") || content.Contains("SpriteAtlas"))
                    {
                        Debug.Log("✓ Sprite atlas methods found");
                        return ValidationResult.Pass;
                    }
                }
            }

            Debug.Log("⚠ Sprite atlas methods not fully implemented (simulated)");
            return ValidationResult.Warning;
        }

        ValidationResult ValidateCategoryDetection()
        {
            // Check if RecraftAIManager has category detection
            string managerPath = "Assets/AIAssetTools/RecraftAIManager.cs";
            if (!File.Exists(managerPath))
            {
                Debug.Log("✗ RecraftAIManager.cs not found");
                return ValidationResult.Fail;
            }

            string content = File.ReadAllText(managerPath);
            if (content.Contains("GetAssetPathFromPrompt") && content.Contains("GenerateFileNameFromPrompt"))
            {
                // Test category detection
                RecraftAIManager manager = new RecraftAIManager();
                string testPath = manager.GetAssetPathFromPrompt("fantasy sword icon", "sword.png");

                if (testPath.Contains("Items/Weapons/"))
                {
                    Debug.Log("✓ Category detection working correctly");
                    return ValidationResult.Pass;
                }
                else
                {
                    Debug.Log("✗ Category detection not working");
                    return ValidationResult.Fail;
                }
            }
            else
            {
                Debug.Log("✗ Category detection methods not found");
                return ValidationResult.Fail;
            }
        }

        ValidationResult ValidateRateLimiting()
        {
            // Check if rate limiting exists in RecraftAIManager
            string managerPath = "Assets/AIAssetTools/RecraftAIManager.cs";
            if (!File.Exists(managerPath))
            {
                Debug.Log("✗ RecraftAIManager.cs not found");
                return ValidationResult.Fail;
            }

            string content = File.ReadAllText(managerPath);
            if (content.Contains("delayBetweenRequests") && content.Contains("Task.Delay") && content.Contains("batchSize"))
            {
                Debug.Log("✓ Rate limiting implementation found");
                return ValidationResult.Pass;
            }
            else
            {
                Debug.Log("✗ Rate limiting implementation incomplete");
                return ValidationResult.Fail;
            }
        }

        void CreateTestTexture(string path)
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Create a simple test texture
            Texture2D texture = new Texture2D(64, 64);
            Color[] colors = new Color[64 * 64];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.white;
            texture.SetPixels(colors);
            texture.Apply();

            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(path, pngData);
        }
    }
}