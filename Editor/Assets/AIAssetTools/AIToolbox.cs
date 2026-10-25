using UnityEngine;
using UnityEditor;

namespace AIAssetTools
{
    /// <summary>
    /// Quick access to all AI asset tools
    /// </summary>
    public class AIToolbox : EditorWindow
    {
        [MenuItem("Tools/AI Asset Tools/Toolbox")]
        static void ShowWindow()
        {
            GetWindow<AIToolbox>("AI Toolbox");
        }

        void OnGUI()
        {
            GUILayout.Label("AI Asset Toolbox", EditorStyles.boldLabel);
            GUILayout.Label("Quick access to all AI asset generation tools");

            EditorGUILayout.Space(10);

            // Main pipeline
            GUILayout.Label("Main Pipeline:", EditorStyles.boldLabel);
            if (GUILayout.Button("Asset Pipeline"))
            {
                GetWindow<AssetPipeline>("AI Asset Pipeline");
            }

            EditorGUILayout.Space(5);

            // Individual tools
            GUILayout.Label("Individual Tools:", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Asset Generator"))
            {
                GetWindow<AssetGenerator>("AI Asset Generator");
            }

            if (GUILayout.Button("Goonzu Generator"))
            {
                GetWindow<GoonzuAssetGenerator>("Goonzu Asset Generator");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Batch Importer"))
            {
                GetWindow<BatchAssetImporter>("Batch Asset Importer");
            }

            if (GUILayout.Button("Demo"))
            {
                GetWindow<AssetPipelineDemo>("Asset Pipeline Demo");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Validator"))
            {
                GetWindow<AssetPipelineValidator>("Pipeline Validator");
            }

            if (GUILayout.Button("Organizer"))
            {
                // AssetOrganizer is static, so show usage info
                ShowOrganizerInfo();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Quick actions
            GUILayout.Label("Quick Actions:", EditorStyles.boldLabel);

            if (GUILayout.Button("Setup New Project"))
            {
                SetupNewProject();
            }

            if (GUILayout.Button("Generate Core Assets"))
            {
                GenerateCoreAssets();
            }

            if (GUILayout.Button("Generate Goonzu Assets"))
            {
                GenerateGoonzuAssets();
            }

            if (GUILayout.Button("Import All Assets"))
            {
                ImportAllAssets();
            }

            if (GUILayout.Button("Create Documentation"))
            {
                CreateDocumentation();
            }

            EditorGUILayout.Space(10);

            // Help
            GUILayout.Label("Help:", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "1. Use 'Setup New Project' to initialize folders and placeholders\n" +
                "2. Use 'Goonzu Generator' to create medieval Goonzu-style assets\n" +
                "3. Generate assets using AI tools with prompts from .txt files\n" +
                "4. Place PNGs in appropriate folders for auto-import\n" +
                "5. Use 'Import All Assets' to batch process everything\n" +
                "6. Use 'Demo' to see all features in action\n" +
                "7. Use 'Validator' to check if everything is working\n" +
                "8. Check Docs/AIAssetPlatforms/ for platform documentation",
                MessageType.Info
            );
        }

        void SetupNewProject()
        {
            AssetOrganizer.CreateAssetFolders();
            AssetPlaceholderGenerator.GenerateAllPlaceholders();
            Debug.Log("New project setup complete! Check Assets/GoonzuGame/ for folders and placeholders.");
        }

        void GenerateCoreAssets()
        {
            // Find or create Recraft manager
            RecraftAIManager manager = FindObjectOfType<RecraftAIManager>();
            if (manager == null)
            {
                GameObject obj = new GameObject("RecraftAIManager");
                manager = obj.AddComponent<RecraftAIManager>();
            }

            manager.GenerateAllCharacters();
            Debug.Log("Core asset generation started. Check console for progress.");
        }

        void GenerateGoonzuAssets()
        {
            // Open the Goonzu Asset Generator window
            GetWindow<GoonzuAssetGenerator>("Goonzu Asset Generator");
            Debug.Log("Goonzu Asset Generator opened. Use it to create medieval Goonzu-style assets.");
        }

        void ImportAllAssets()
        {
            string[] pngFiles = System.IO.Directory.GetFiles("Assets/GoonzuGame", "*.png", System.IO.SearchOption.AllDirectories);

            foreach (string pngFile in pngFiles)
            {
                AssetDatabase.ImportAsset(pngFile, ImportAssetOptions.ForceUpdate);
            }

            AssetDatabase.Refresh();
            Debug.Log($"Imported {pngFiles.Length} assets");
        }

        void CreateDocumentation()
        {
            Application.OpenURL("file://" + System.IO.Path.GetFullPath("Docs/README.md"));
        }

        void ShowOrganizerInfo()
        {
            EditorUtility.DisplayDialog(
                "Asset Organizer",
                "AssetOrganizer is a static utility class. Use these methods:\n\n" +
                "- AssetOrganizer.CreateAssetFolders()\n" +
                "- AssetOrganizer.MoveAssetToCategory(path, category)\n" +
                "- AssetOrganizer.RenameAssetWithConvention(path, prefix)\n\n" +
                "Access via code or create a custom editor script.",
                "OK"
            );
        }
    }
}