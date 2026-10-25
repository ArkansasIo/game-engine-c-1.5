using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AIAssetTools
{
    /// <summary>
    /// Demonstration of all AI asset pipeline features
    /// </summary>
    public class AssetPipelineDemo : EditorWindow
    {
        [MenuItem("Tools/AI Asset Tools/Pipeline Demo")]
        static void ShowWindow()
        {
            GetWindow<AssetPipelineDemo>("Asset Pipeline Demo");
        }

        private Vector2 scrollPos;
        private bool demoRunning = false;
        private string demoStatus = "Ready to start demo";

        void OnGUI()
        {
            GUILayout.Label("AI Asset Pipeline Demo", EditorStyles.boldLabel);
            GUILayout.Label("Demonstrate all key features in action");

            EditorGUILayout.Space();

            // Status
            EditorGUILayout.LabelField("Status:", demoStatus);
            if (demoRunning)
            {
                EditorGUILayout.HelpBox("Demo is running... Check console for detailed output", MessageType.Info);
            }

            EditorGUILayout.Space();

            // Feature demonstrations
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            GUILayout.Label("Feature Demonstrations:", EditorStyles.boldLabel);

            if (GUILayout.Button("1. Automatic Import Demo"))
            {
                RunAutomaticImportDemo();
            }

            if (GUILayout.Button("2. Batch Generation Demo"))
            {
                RunBatchGenerationDemo();
            }

            if (GUILayout.Button("3. Asset Validation Demo"))
            {
                RunAssetValidationDemo();
            }

            if (GUILayout.Button("4. Sprite Atlas Demo"))
            {
                RunSpriteAtlasDemo();
            }

            if (GUILayout.Button("5. Category Detection Demo"))
            {
                RunCategoryDetectionDemo();
            }

            if (GUILayout.Button("6. Rate Limiting Demo"))
            {
                RunRateLimitingDemo();
            }

            EditorGUILayout.Space();

            // Full pipeline demo
            GUILayout.Label("Complete Pipeline Demo:", EditorStyles.boldLabel);
            if (GUILayout.Button("Run Full Pipeline Demo"))
            {
                RunFullPipelineDemo();
            }

            if (GUILayout.Button("Create Test Assets"))
            {
                CreateTestAssets();
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            // Help
            EditorGUILayout.HelpBox(
                "This demo shows how all AI asset pipeline features work together:\n\n" +
                "1. Automatic Import: PNGs placed in folders get auto-configured\n" +
                "2. Batch Generation: Multiple assets generated with API simulation\n" +
                "3. Asset Validation: Checks import success and sprite creation\n" +
                "4. Sprite Atlases: Performance optimization for UI elements\n" +
                "5. Category Detection: Smart file naming from prompts\n" +
                "6. Rate Limiting: Prevents API overload during generation\n\n" +
                "Use 'Create Test Assets' first to set up sample files.",
                MessageType.Info
            );
        }

        async void RunAutomaticImportDemo()
        {
            demoRunning = true;
            demoStatus = "Running Automatic Import Demo...";

            Debug.Log("=== AUTOMATIC IMPORT DEMO ===");
            Debug.Log("This demo shows how PNGs placed in category folders get automatically configured as sprites");

            // Create a test PNG in Characters folder
            string testPath = "Assets/GoonzuGame/Characters/demo_character.png";
            CreateTestPNG(testPath);

            Debug.Log($"Created test PNG: {testPath}");
            Debug.Log("AssetImporter should automatically configure this as a pixel-perfect sprite (64x64, Point filter)");

            // Force import to trigger AssetImporter
            AssetDatabase.ImportAsset(testPath, ImportAssetOptions.ForceUpdate);

            // Check the import settings
            TextureImporter importer = AssetImporter.GetAtPath(testPath) as TextureImporter;
            if (importer != null)
            {
                Debug.Log($"Import Settings Applied:");
                Debug.Log($"- Texture Type: {importer.textureType}");
                Debug.Log($"- Sprite Mode: {importer.spriteImportMode}");
                Debug.Log($"- Filter Mode: {importer.filterMode}");
                Debug.Log($"- Pixels Per Unit: {importer.spritePixelsPerUnit}");
                Debug.Log($"- Mipmaps: {importer.mipmapEnabled}");
            }

            await Task.Delay(1000);
            demoStatus = "Automatic Import Demo Complete";
            demoRunning = false;
        }

        async void RunBatchGenerationDemo()
        {
            demoRunning = true;
            demoStatus = "Running Batch Generation Demo...";

            Debug.Log("=== BATCH GENERATION DEMO ===");
            Debug.Log("This demo shows batch generation of multiple assets with simulated API calls");

            // Create Recraft manager if needed
            RecraftAIManager manager = FindObjectOfType<RecraftAIManager>();
            if (manager == null)
            {
                GameObject obj = new GameObject("RecraftAIManager_Demo");
                manager = obj.AddComponent<RecraftAIManager>();
            }

            // Generate a batch of test assets
            List<string> testPrompts = new List<string>
            {
                "fantasy sword icon, game inventory item, bright colors, clean outline, transparent background, anime-inspired fantasy MMORPG style",
                "fantasy health potion bottle icon, red liquid, anime-inspired fantasy MMORPG style",
                "fantasy gold coin icon, currency, anime-inspired fantasy MMORPG style"
            };

            Debug.Log($"Starting batch generation of {testPrompts.Count} assets...");
            manager.BatchGenerateAssets(testPrompts);

            await Task.Delay(5000); // Wait for simulated generation
            demoStatus = "Batch Generation Demo Complete";
            demoRunning = false;
        }

        void RunAssetValidationDemo()
        {
            demoRunning = true;
            demoStatus = "Running Asset Validation Demo...";

            Debug.Log("=== ASSET VALIDATION DEMO ===");
            Debug.Log("This demo validates all assets in the GoonzuGame folder");

            string[] pngFiles = Directory.GetFiles("Assets/GoonzuGame", "*.png", SearchOption.AllDirectories);
            int validAssets = 0;
            int invalidAssets = 0;

            Debug.Log($"Found {pngFiles.Length} PNG files to validate:");

            foreach (string pngFile in pngFiles)
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(pngFile);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(pngFile);

                string fileName = Path.GetFileName(pngFile);
                if (texture != null && sprite != null)
                {
                    validAssets++;
                    Debug.Log($"✓ {fileName} - Valid (Texture: {texture.width}x{texture.height}, Sprite: {sprite.pixelsPerUnit} PPU)");
                }
                else
                {
                    invalidAssets++;
                    Debug.Log($"✗ {fileName} - Invalid (Texture: {texture != null}, Sprite: {sprite != null})");
                }
            }

            Debug.Log($"Validation Results: {validAssets} valid, {invalidAssets} invalid assets");
            demoStatus = "Asset Validation Demo Complete";
            demoRunning = false;
        }

        void RunSpriteAtlasDemo()
        {
            demoRunning = true;
            demoStatus = "Running Sprite Atlas Demo...";

            Debug.Log("=== SPRITE ATLAS DEMO ===");
            Debug.Log("This demo shows sprite atlas creation for performance optimization");

            // Check for UI assets
            string[] uiIcons = Directory.GetFiles("Assets/GoonzuGame/UI/Icons", "*.png", SearchOption.AllDirectories);
            string[] uiPanels = Directory.GetFiles("Assets/GoonzuGame/UI/Panels", "*.png", SearchOption.AllDirectories);

            Debug.Log($"Found {uiIcons.Length} UI icons and {uiPanels.Length} UI panels");

            if (uiIcons.Length > 0 || uiPanels.Length > 0)
            {
                Debug.Log("Creating UI sprite atlas...");
                // In a real implementation, this would create actual sprite atlases
                Debug.Log("✓ UI Sprite Atlas created (simulated)");
                Debug.Log("✓ Character Sprite Atlas created (simulated)");
                Debug.Log("✓ Item Sprite Atlas created (simulated)");
            }
            else
            {
                Debug.Log("No UI assets found. Create some UI assets first with 'Create Test Assets'");
            }

            demoStatus = "Sprite Atlas Demo Complete";
            demoRunning = false;
        }

        void RunCategoryDetectionDemo()
        {
            demoRunning = true;
            demoStatus = "Running Category Detection Demo...";

            Debug.Log("=== CATEGORY DETECTION DEMO ===");
            Debug.Log("This demo shows smart file naming and path assignment from prompts");

            RecraftAIManager manager = new RecraftAIManager();

            // Test prompts and their expected categories
            Dictionary<string, string> testPrompts = new Dictionary<string, string>
            {
                {"fantasy sword icon, game inventory item", "Items/Weapons/"},
                {"male fantasy adventurer, light armor", "Characters/"},
                {"isometric medieval town house", "Buildings/"},
                {"seamless grass tile texture", "Tiles/"},
                {"fantasy health potion bottle icon", "Items/Consumables/"},
                {"fantasy horse mount side view", "Creatures/Mounts/"},
                {"fantasy MMORPG inventory window frame", "UI/Panels/"}
            };

            foreach (var kvp in testPrompts)
            {
                string fileName = manager.GenerateFileNameFromPrompt(kvp.Key);
                string fullPath = manager.GetAssetPathFromPrompt(kvp.Key, fileName);

                Debug.Log($"Prompt: '{kvp.Key}'");
                Debug.Log($"  → File: {fileName}");
                Debug.Log($"  → Path: {fullPath}");
                Debug.Log($"  → Expected Category: {kvp.Value}");
                Debug.Log("");
            }

            demoStatus = "Category Detection Demo Complete";
            demoRunning = false;
        }

        async void RunRateLimitingDemo()
        {
            demoRunning = true;
            demoStatus = "Running Rate Limiting Demo...";

            Debug.Log("=== RATE LIMITING DEMO ===");
            Debug.Log("This demo shows rate limiting to prevent API overload");

            RecraftAIManager manager = FindObjectOfType<RecraftAIManager>();
            if (manager == null)
            {
                GameObject obj = new GameObject("RecraftAIManager_Demo");
                manager = obj.AddComponent<RecraftAIManager>();
            }

            // Set a short delay for demo
            float originalDelay = manager.delayBetweenRequests;
            manager.delayBetweenRequests = 0.5f;

            List<string> rateLimitTest = new List<string>
            {
                "test asset 1",
                "test asset 2",
                "test asset 3",
                "test asset 4",
                "test asset 5"
            };

            Debug.Log($"Generating {rateLimitTest.Count} assets with {manager.delayBetweenRequests}s delay between requests...");

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            manager.BatchGenerateAssets(rateLimitTest);

            // Wait for completion (simulated)
            await Task.Delay((int)((rateLimitTest.Count * manager.delayBetweenRequests + 3) * 1000));

            stopwatch.Stop();
            Debug.Log($"Rate limiting demo completed in {stopwatch.Elapsed.TotalSeconds:F1} seconds");
            Debug.Log($"Expected minimum time: {(rateLimitTest.Count * manager.delayBetweenRequests):F1} seconds");

            // Restore original delay
            manager.delayBetweenRequests = originalDelay;

            demoStatus = "Rate Limiting Demo Complete";
            demoRunning = false;
        }

        async void RunFullPipelineDemo()
        {
            demoRunning = true;
            demoStatus = "Running Full Pipeline Demo...";

            Debug.Log("=== FULL PIPELINE DEMO ===");
            Debug.Log("This demo runs the complete asset pipeline from setup to optimization");

            // Step 1: Setup
            demoStatus = "Step 1: Setting up folders and placeholders...";
            await Task.Delay(500);
            AssetOrganizer.CreateAssetFolders();
            AssetPlaceholderGenerator.GenerateAllPlaceholders();
            Debug.Log("✓ Setup complete");

            // Step 2: Generate
            demoStatus = "Step 2: Generating sample assets...";
            await Task.Delay(500);
            CreateTestAssets();
            Debug.Log("✓ Generation complete");

            // Step 3: Import
            demoStatus = "Step 3: Importing and configuring assets...";
            await Task.Delay(500);
            string[] pngFiles = Directory.GetFiles("Assets/GoonzuGame", "*.png", SearchOption.AllDirectories);
            foreach (string pngFile in pngFiles)
            {
                AssetDatabase.ImportAsset(pngFile, ImportAssetOptions.ForceUpdate);
            }
            AssetDatabase.Refresh();
            Debug.Log($"✓ Imported {pngFiles.Length} assets");

            // Step 4: Validate
            demoStatus = "Step 4: Validating assets...";
            await Task.Delay(500);
            RunAssetValidationDemo();
            Debug.Log("✓ Validation complete");

            // Step 5: Optimize
            demoStatus = "Step 5: Creating atlases and optimizing...";
            await Task.Delay(500);
            RunSpriteAtlasDemo();
            Debug.Log("✓ Optimization complete");

            demoStatus = "Full Pipeline Demo Complete!";
            demoRunning = false;
            Debug.Log("🎉 Full pipeline demo completed successfully!");
        }

        void CreateTestAssets()
        {
            Debug.Log("Creating test assets for demonstration...");

            // Create test PNGs in different categories
            CreateTestPNG("Assets/GoonzuGame/Characters/test_player.png");
            CreateTestPNG("Assets/GoonzuGame/Buildings/test_house.png");
            CreateTestPNG("Assets/GoonzuGame/Tiles/test_grass.png");
            CreateTestPNG("Assets/GoonzuGame/Items/Weapons/test_sword.png");
            CreateTestPNG("Assets/GoonzuGame/UI/Icons/test_coin.png");

            AssetDatabase.Refresh();
            Debug.Log("✓ Test assets created");
        }

        void CreateTestPNG(string path)
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Create a simple colored texture
            Texture2D texture = new Texture2D(64, 64);
            Color[] colors = new Color[64 * 64];

            // Create a simple pattern
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    if ((x + y) % 2 == 0)
                        colors[y * 64 + x] = Color.red;
                    else
                        colors[y * 64 + x] = Color.blue;
                }
            }

            texture.SetPixels(colors);
            texture.Apply();

            // Save as PNG
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(path, pngData);

            Debug.Log($"Created test PNG: {path}");
        }
    }
}