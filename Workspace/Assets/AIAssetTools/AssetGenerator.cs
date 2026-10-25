using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AIAssetTools
{
    /// <summary>
    /// Handles batch generation of assets using AI platforms
    /// </summary>
    public class AssetGenerator : EditorWindow
    {
        [MenuItem("Tools/AI Asset Tools/Asset Generator")]
        static void ShowWindow()
        {
            GetWindow<AssetGenerator>("AI Asset Generator");
        }

        private AIPlatform selectedPlatform = AIPlatform.RecraftAI;
        private string apiKey = "";
        private bool useBatchMode = true;
        private Vector2 scrollPos;
        private Dictionary<string, bool> selectedAssets = new Dictionary<string, bool>();

        enum AIPlatform
        {
            RecraftAI,
            ScenarioAI,
            MeshyAI,
            PixelcutAI
        }

        void OnEnable()
        {
            InitializeAssetSelection();
        }

        void InitializeAssetSelection()
        {
            selectedAssets.Clear();
            string[] txtFiles = Directory.GetFiles("Assets/GoonzuGame", "*.txt", SearchOption.AllDirectories);

            foreach (string file in txtFiles)
            {
                string relativePath = file.Replace("Assets/GoonzuGame/", "").Replace(".txt", "");
                selectedAssets[relativePath] = false;
            }
        }

        void OnGUI()
        {
            GUILayout.Label("AI Asset Generator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Platform selection
            selectedPlatform = (AIPlatform)EditorGUILayout.EnumPopup("AI Platform", selectedPlatform);

            // API Key
            apiKey = EditorGUILayout.PasswordField("API Key", apiKey);

            // Batch mode
            useBatchMode = EditorGUILayout.Toggle("Batch Generation", useBatchMode);

            GUILayout.Space(10);
            GUILayout.Label("Select Assets to Generate:", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(300));

            // Category headers and checkboxes
            DrawAssetCategory("Characters", "Characters/");
            DrawAssetCategory("Buildings", "Buildings/");
            DrawAssetCategory("Tiles", "Tiles/");
            DrawAssetCategory("Items", "Items/");
            DrawAssetCategory("Creatures", "Creatures/");
            DrawAssetCategory("UI", "UI/");

            EditorGUILayout.EndScrollView();

            GUILayout.Space(10);

            if (GUILayout.Button("Generate Selected Assets"))
            {
                GenerateSelectedAssets();
            }

            if (GUILayout.Button("Export Prompts to File"))
            {
                ExportPromptsToFile();
            }

            if (GUILayout.Button("Refresh Asset List"))
            {
                InitializeAssetSelection();
            }
        }

        void DrawAssetCategory(string categoryName, string pathPrefix)
        {
            bool hasAssetsInCategory = false;
            foreach (var kvp in selectedAssets)
            {
                if (kvp.Key.StartsWith(pathPrefix))
                {
                    hasAssetsInCategory = true;
                    break;
                }
            }

            if (!hasAssetsInCategory) return;

            GUILayout.Label(categoryName, EditorStyles.boldLabel);

            foreach (var kvp in selectedAssets)
            {
                if (kvp.Key.StartsWith(pathPrefix))
                {
                    string displayName = kvp.Key.Replace(pathPrefix, "");
                    selectedAssets[kvp.Key] = EditorGUILayout.Toggle(displayName, kvp.Value);
                }
            }

            GUILayout.Space(5);
        }

        void GenerateSelectedAssets()
        {
            List<string> selectedPrompts = new List<string>();

            foreach (var kvp in selectedAssets)
            {
                if (kvp.Value)
                {
                    string txtPath = $"Assets/GoonzuGame/{kvp.Key}.txt";
                    if (File.Exists(txtPath))
                    {
                        string content = File.ReadAllText(txtPath);
                        string prompt = ExtractPromptFromContent(content);
                        if (!string.IsNullOrEmpty(prompt))
                        {
                            selectedPrompts.Add(prompt);
                        }
                    }
                }
            }

            if (selectedPrompts.Count == 0)
            {
                Debug.LogWarning("No assets selected for generation");
                return;
            }

            Debug.Log($"Starting generation of {selectedPrompts.Count} assets using {selectedPlatform}");

            // Here you would integrate with actual AI APIs
            // For now, we'll just log the prompts
            foreach (string prompt in selectedPrompts)
            {
                Debug.Log($"Would generate: {prompt}");
            }

            // Simulate API call
            SimulateAssetGeneration(selectedPrompts);
        }

        void ExportPromptsToFile()
        {
            string filePath = "Assets/GoonzuGame/selected_prompts.txt";
            List<string> selectedPrompts = new List<string>();

            foreach (var kvp in selectedAssets)
            {
                if (kvp.Value)
                {
                    string txtPath = $"Assets/GoonzuGame/{kvp.Key}.txt";
                    if (File.Exists(txtPath))
                    {
                        string content = File.ReadAllText(txtPath);
                        selectedPrompts.Add($"// {kvp.Key}");
                        selectedPrompts.Add(ExtractPromptFromContent(content));
                        selectedPrompts.Add("");
                    }
                }
            }

            File.WriteAllLines(filePath, selectedPrompts.ToArray());
            AssetDatabase.Refresh();

            Debug.Log($"Exported {selectedPrompts.Count/3} prompts to {filePath}");
        }

        string ExtractPromptFromContent(string content)
        {
            string[] lines = content.Split('\n');
            foreach (string line in lines)
            {
                if (line.Trim().StartsWith("# Generate using:"))
                {
                    return line.Replace("# Generate using:", "").Trim();
                }
            }
            return content; // Fallback
        }

        async void SimulateAssetGeneration(List<string> prompts)
        {
            // This is where you would implement actual API calls
            // For demonstration, we'll just wait and log

            Debug.Log("Simulating asset generation...");

            await Task.Delay(2000); // Simulate API delay

            Debug.Log($"Generated {prompts.Count} assets successfully!");
            Debug.Log("Note: In a real implementation, this would download and import the generated images.");
        }
    }
}