using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using GoonzuGame.Core;

namespace AIAssetTools
{
    /// <summary>
    /// Manager for Recraft AI assets (consistent game assets)
    /// </summary>
    public class RecraftAIManager : AIAssetManager
    {
        [Header("Recraft AI Specific")]
        public bool transparentBackground = true;
        public string outputFormat = "PNG";
        public string apiKey = "";
        public List<string> promptTemplates = new List<string>();

        [Header("Generation Settings")]
        public int batchSize = 5;
        public float delayBetweenRequests = 1f;

        protected override void InitializePlatform()
        {
            platformName = "Recraft AI";
            // Initialize with GoonZu style prompts
            promptTemplates.Add("anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background");
            Debug.Log("Recraft AI Manager initialized");
        }

        public void GenerateIconSet(string category)
        {
            string prompt = $"{category} icon set, {promptTemplates[0]}";
            Debug.Log($"Generating icon set: {prompt}");
            GenerateAsset(prompt, $"Assets/GoonzuGame/UI/Icons/{category}_set.png");
        }

        public async void BatchGenerateAssets(List<string> prompts)
        {
            Debug.Log($"Starting batch generation of {prompts.Count} assets");

            for (int i = 0; i < prompts.Count; i += batchSize)
            {
                List<string> batch = prompts.GetRange(i, Mathf.Min(batchSize, prompts.Count - i));

                foreach (string prompt in batch)
                {
                    // Generate asset for each prompt
                    string fileName = GenerateFileNameFromPrompt(prompt);
                    string filePath = GetAssetPathFromPrompt(prompt, fileName);

                    GenerateAsset(prompt, filePath);

                    // Delay between requests to avoid rate limiting
                    await Task.Delay((int)(delayBetweenRequests * 1000));
                }

                Debug.Log($"Completed batch {i/batchSize + 1}");
            }

            Debug.Log("Batch generation complete!");
        }

        public void GenerateAsset(string prompt, string outputPath = null)
        {
            if (string.IsNullOrEmpty(outputPath))
            {
                string fileName = GenerateFileNameFromPrompt(prompt);
                outputPath = GetAssetPathFromPrompt(prompt, fileName);
            }

            Debug.Log($"Generating asset: {prompt}");
            Debug.Log($"Output path: {outputPath}");

            // In a real implementation, this would call the Recraft AI API
            // For now, we'll simulate the generation
            SimulateAssetGeneration(prompt, outputPath);
        }

        private async void SimulateAssetGeneration(string prompt, string outputPath)
        {
            // Simulate API call delay
            await Task.Delay(3000);

            // Create a placeholder texture (in real implementation, this would be downloaded)
            CreatePlaceholderTexture(outputPath);

            Debug.Log($"Simulated generation complete: {outputPath}");
        }

        private void CreatePlaceholderTexture(string path)
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Create a simple colored texture as placeholder
            Texture2D texture = new Texture2D(64, 64);
            Color[] colors = new Color[64 * 64];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(Random.value, Random.value, Random.value, 1f);
            }

            texture.SetPixels(colors);
            texture.Apply();

            // Save as PNG
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(path, pngData);

            // Import the asset
            AssetDatabase.ImportAsset(path);

            Debug.Log($"Created placeholder texture: {path}");
        }

        private string GenerateFileNameFromPrompt(string prompt)
        {
            // Extract key terms from prompt to create filename
            string[] keywords = prompt.Split(new char[] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            string fileName = "";

            foreach (string keyword in keywords)
            {
                string cleanKeyword = keyword.ToLower().Replace("-", "_");
                if (cleanKeyword.Length > 2 && !cleanKeyword.Contains("style") && !cleanKeyword.Contains("color"))
                {
                    fileName += cleanKeyword + "_";
                    if (fileName.Length > 20) break; // Limit length
                }
            }

            return fileName.TrimEnd('_') + ".png";
        }

        private string GetAssetPathFromPrompt(string prompt, string fileName)
        {
            string basePath = "Assets/GoonzuGame/";

            // Determine category based on prompt content
            if (prompt.Contains("character") || prompt.Contains("NPC") || prompt.Contains("merchant"))
                return basePath + "Characters/" + fileName;
            else if (prompt.Contains("building") || prompt.Contains("house") || prompt.Contains("shop"))
                return basePath + "Buildings/" + fileName;
            else if (prompt.Contains("tile") || prompt.Contains("texture") || prompt.Contains("ground"))
                return basePath + "Tiles/" + fileName;
            else if (prompt.Contains("icon") || prompt.Contains("inventory") || prompt.Contains("equipment"))
                return basePath + "Items/" + fileName;
            else if (prompt.Contains("mount") || prompt.Contains("monster") || prompt.Contains("creature"))
                return basePath + "Creatures/" + fileName;
            else if (prompt.Contains("UI") || prompt.Contains("panel") || prompt.Contains("window") || prompt.Contains("bar"))
                return basePath + "UI/" + fileName;
            else
                return basePath + "Generated/" + fileName;
        }

        // Editor integration
        [ContextMenu("Test Generate Sample Asset")]
        void TestGenerateSampleAsset()
        {
            string testPrompt = "fantasy sword icon, game inventory item, bright colors, clean outline, transparent background, anime-inspired fantasy MMORPG style";
            GenerateAsset(testPrompt);
        }

        [ContextMenu("Generate All Character Assets")]
        void GenerateAllCharacters()
        {
            List<string> characterPrompts = new List<string>
            {
                "male fantasy adventurer, light armor, standing pose, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background",
                "female fantasy adventurer, leather armor, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background",
                "friendly town merchant NPC, medieval clothing, market trader, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
            };

            BatchGenerateAssets(characterPrompts);
        }
    }
}