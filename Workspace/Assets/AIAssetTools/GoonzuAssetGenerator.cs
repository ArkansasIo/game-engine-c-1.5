using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AIAssetTools
{
    /// <summary>
    /// Specialized generator for medieval Goonzu-style 2D and 3D assets
    /// </summary>
    public class GoonzuAssetGenerator : EditorWindow
    {
        [MenuItem("Tools/AI Asset Tools/Goonzu Asset Generator")]
        static void ShowWindow()
        {
            GetWindow<GoonzuAssetGenerator>("Goonzu Asset Generator");
        }

        private Vector2 scrollPos;
        private bool generate2D = true;
        private bool generate3D = true;
        private int assetCount = 5;
        private bool isGenerating = false;

        // Medieval Goonzu style prompts
        private readonly string styleBase = "anime-inspired fantasy MMORPG style, medieval Goonzu, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset";

        void OnGUI()
        {
            GUILayout.Label("Medieval Goonzu Asset Generator", EditorStyles.boldLabel);
            GUILayout.Label("Generate 2D sprites and 3D models in authentic Goonzu style");

            EditorGUILayout.Space();

            // Generation options
            generate2D = EditorGUILayout.Toggle("Generate 2D Assets", generate2D);
            generate3D = EditorGUILayout.Toggle("Generate 3D Assets", generate3D);
            assetCount = EditorGUILayout.IntSlider("Assets per Category", assetCount, 1, 20);

            EditorGUILayout.Space();

            if (isGenerating)
            {
                EditorGUILayout.HelpBox("Generating assets... Check console for progress.", MessageType.Info);
                if (GUILayout.Button("Cancel Generation"))
                {
                    isGenerating = false;
                }
            }
            else
            {
                if (GUILayout.Button("Generate All Goonzu Assets"))
                {
                    GenerateAllGoonzuAssets();
                }

                EditorGUILayout.Space();

                // Category-specific generation
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                GUILayout.Label("Generate by Category:", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Characters"))
                {
                    GenerateCharacterAssets();
                }
                if (GUILayout.Button("Buildings"))
                {
                    GenerateBuildingAssets();
                }
                if (GUILayout.Button("Items"))
                {
                    GenerateItemAssets();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Creatures"))
                {
                    GenerateCreatureAssets();
                }
                if (GUILayout.Button("UI Elements"))
                {
                    GenerateUIAssets();
                }
                if (GUILayout.Button("Tiles"))
                {
                    GenerateTileAssets();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();
            }

            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "This generator creates medieval Goonzu-style assets using the established AI pipeline.\n\n" +
                "2D Assets: PNG sprites with proper import settings\n" +
                "3D Assets: FBX models ready for Unity (simulated)\n\n" +
                "Assets are placed in Assets/GoonzuGame/ folders automatically.",
                MessageType.Info
            );
        }

        async void GenerateAllGoonzuAssets()
        {
            isGenerating = true;
            Debug.Log("=== GENERATING ALL MEDIEVAL GOONZU ASSETS ===");

            try
            {
                if (generate2D)
                {
                    await GenerateCharacterAssets();
                    await GenerateBuildingAssets();
                    await GenerateItemAssets();
                    await GenerateCreatureAssets();
                    await GenerateUIAssets();
                    await GenerateTileAssets();
                }

                if (generate3D)
                {
                    await Generate3DAssets();
                }

                Debug.Log("✓ All Goonzu assets generated successfully!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Asset generation failed: {e.Message}");
            }

            isGenerating = false;
        }

        async Task GenerateCharacterAssets()
        {
            Debug.Log("Generating medieval Goonzu character assets...");

            List<string> characterPrompts = new List<string>
            {
                $"male medieval adventurer, fantasy armor, heroic pose, {styleBase}, transparent background",
                $"female medieval merchant, traveling clothes, friendly expression, {styleBase}, transparent background",
                $"medieval blacksmith NPC, apron and hammer, muscular build, {styleBase}, transparent background",
                $"medieval town guard, plate armor, spear and shield, {styleBase}, transparent background",
                $"medieval farmer NPC, straw hat, simple clothes, {styleBase}, transparent background",
                $"medieval noble character, elegant robe, dignified pose, {styleBase}, transparent background",
                $"medieval warrior class, battle-worn armor, determined expression, {styleBase}, transparent background",
                $"medieval mage character, mystical robe, staff, magical aura, {styleBase}, transparent background"
            };

            await GenerateAssetBatch(characterPrompts.GetRange(0, Mathf.Min(assetCount, characterPrompts.Count)), "Characters");
        }

        async Task GenerateBuildingAssets()
        {
            Debug.Log("Generating medieval Goonzu building assets...");

            List<string> buildingPrompts = new List<string>
            {
                $"medieval fantasy town house, timber frame architecture, thatched roof, {styleBase}",
                $"medieval blacksmith shop, stone walls, forge chimney, anvil outside, {styleBase}",
                $"medieval market stall, colorful canopy, wooden structure, goods display, {styleBase}",
                $"medieval tavern building, sign with mug, welcoming entrance, {styleBase}",
                $"medieval castle tower, stone walls, battlements, flags, {styleBase}",
                $"medieval church building, stained glass windows, spire, {styleBase}",
                $"medieval merchant guild hall, ornate architecture, banners, {styleBase}",
                $"medieval stable building, wooden structure, hay bales, {styleBase}"
            };

            await GenerateAssetBatch(buildingPrompts.GetRange(0, Mathf.Min(assetCount, buildingPrompts.Count)), "Buildings");
        }

        async Task GenerateItemAssets()
        {
            Debug.Log("Generating medieval Goonzu item assets...");

            List<string> weaponPrompts = new List<string>
            {
                $"medieval broadsword, ornate hilt, fantasy design, {styleBase}, transparent background",
                $"medieval steel shield, heraldic emblem, battle-scarred, {styleBase}, transparent background",
                $"medieval war hammer, spiked head, leather grip, {styleBase}, transparent background",
                $"medieval longbow, carved wood, string taut, {styleBase}, transparent background",
                $"medieval mage staff, crystal orb, glowing runes, {styleBase}, transparent background"
            };

            List<string> armorPrompts = new List<string>
            {
                $"medieval knight helmet, visor down, ornate design, {styleBase}, transparent background",
                $"medieval chainmail armor, metal rings, fantasy style, {styleBase}, transparent background",
                $"medieval leather boots, sturdy design, travel-worn, {styleBase}, transparent background",
                $"medieval noble cape, rich fabric, gold trim, {styleBase}, transparent background"
            };

            List<string> materialPrompts = new List<string>
            {
                $"iron ore chunks, metallic texture, mining resource, {styleBase}, transparent background",
                $"wooden logs bundle, fresh cut, crafting material, {styleBase}, transparent background",
                $"leather hides, tanned, crafting material, {styleBase}, transparent background",
                $"gold coins pile, currency, shiny metal, {styleBase}, transparent background",
                $"wheat bundle, farming resource, golden stalks, {styleBase}, transparent background"
            };

            await GenerateAssetBatch(weaponPrompts.GetRange(0, Mathf.Min(assetCount, weaponPrompts.Count)), "Items/Weapons");
            await GenerateAssetBatch(armorPrompts.GetRange(0, Mathf.Min(assetCount, armorPrompts.Count)), "Items/Armor");
            await GenerateAssetBatch(materialPrompts.GetRange(0, Mathf.Min(assetCount, materialPrompts.Count)), "Items/Materials");
        }

        async Task GenerateCreatureAssets()
        {
            Debug.Log("Generating medieval Goonzu creature assets...");

            List<string> mountPrompts = new List<string>
            {
                $"medieval war horse, armored barding, powerful build, {styleBase}, transparent background",
                $"medieval donkey, pack saddle, loyal companion, {styleBase}, transparent background",
                $"medieval fantasy ox, sturdy horns, farm animal, {styleBase}, transparent background",
                $"medieval camel, desert adaptation, merchant mount, {styleBase}, transparent background"
            };

            List<string> monsterPrompts = new List<string>
            {
                $"medieval goblin creature, green skin, sneaky expression, {styleBase}, transparent background",
                $"medieval orc warrior, muscular build, crude armor, {styleBase}, transparent background",
                $"medieval wolf enemy, snarling, forest predator, {styleBase}, transparent background",
                $"medieval slime monster, gelatinous blob, simple enemy, {styleBase}, transparent background",
                $"medieval skeleton warrior, bony frame, rusted sword, {styleBase}, transparent background"
            };

            await GenerateAssetBatch(mountPrompts.GetRange(0, Mathf.Min(assetCount, mountPrompts.Count)), "Creatures/Mounts");
            await GenerateAssetBatch(monsterPrompts.GetRange(0, Mathf.Min(assetCount, monsterPrompts.Count)), "Creatures/Monsters");
        }

        async Task GenerateUIAssets()
        {
            Debug.Log("Generating medieval Goonzu UI assets...");

            List<string> panelPrompts = new List<string>
            {
                $"medieval fantasy inventory window, ornate border, parchment background, {styleBase}, transparent background",
                $"medieval character stats panel, scroll design, gold trim, {styleBase}, transparent background",
                $"medieval quest log window, ancient book design, leather binding, {styleBase}, transparent background",
                $"medieval trade window, merchant scales, coin displays, {styleBase}, transparent background"
            };

            List<string> iconPrompts = new List<string>
            {
                $"medieval gold coin icon, currency symbol, {styleBase}, transparent background",
                $"medieval sword icon, weapon symbol, {styleBase}, transparent background",
                $"medieval shield icon, protection symbol, {styleBase}, transparent background",
                $"medieval potion bottle icon, health symbol, {styleBase}, transparent background",
                $"medieval map icon, navigation symbol, {styleBase}, transparent background",
                $"medieval chest icon, treasure symbol, {styleBase}, transparent background"
            };

            List<string> barPrompts = new List<string>
            {
                $"medieval health bar UI, red liquid, ornate frame, {styleBase}, transparent background",
                $"medieval mana bar UI, blue glow, magical frame, {styleBase}, transparent background",
                $"medieval experience bar UI, gold fill, progress frame, {styleBase}, transparent background"
            };

            await GenerateAssetBatch(panelPrompts.GetRange(0, Mathf.Min(assetCount, panelPrompts.Count)), "UI/Panels");
            await GenerateAssetBatch(iconPrompts.GetRange(0, Mathf.Min(assetCount, iconPrompts.Count)), "UI/Icons");
            await GenerateAssetBatch(barPrompts.GetRange(0, Mathf.Min(assetCount, barPrompts.Count)), "UI/Bars");
        }

        async Task GenerateTileAssets()
        {
            Debug.Log("Generating medieval Goonzu tile assets...");

            List<string> tilePrompts = new List<string>
            {
                $"medieval cobblestone path tile, stone texture, seamless, {styleBase}",
                $"medieval grass field tile, green texture, wildflowers, {styleBase}",
                $"medieval dirt road tile, packed earth, wheel tracks, {styleBase}",
                $"medieval stone floor tile, castle interior, seamless, {styleBase}",
                $"medieval wooden floor tile, tavern interior, planks, {styleBase}",
                $"medieval water edge tile, river bank, flowing water, {styleBase}",
                $"medieval forest ground tile, leaves and moss, natural, {styleBase}",
                $"medieval sand tile, desert texture, golden grains, {styleBase}"
            };

            await GenerateAssetBatch(tilePrompts.GetRange(0, Mathf.Min(assetCount, tilePrompts.Count)), "Tiles");
        }

        async Task Generate3DAssets()
        {
            Debug.Log("Generating medieval Goonzu 3D assets...");

            List<string> modelPrompts = new List<string>
            {
                $"medieval fantasy sword 3D model, ornate hilt, detailed blade, PBR textures, game-ready, {styleBase}",
                $"medieval castle wall section 3D model, stone blocks, battlements, modular design, {styleBase}",
                $"medieval wooden chest 3D model, iron bands, treasure container, openable, {styleBase}",
                $"medieval potion bottle 3D model, glass vial, glowing liquid, detailed, {styleBase}",
                $"medieval stone well 3D model, bucket, rope, village centerpiece, {styleBase}",
                $"medieval armor stand 3D model, display rack, plate armor, decorative, {styleBase}",
                $"medieval wooden cart 3D model, merchant wagon, wheels, goods storage, {styleBase}",
                $"medieval stone throne 3D model, royal seat, ornate carvings, regal, {styleBase}"
            };

            await Generate3DAssetBatch(modelPrompts.GetRange(0, Mathf.Min(assetCount, modelPrompts.Count)), "3DAssets");
        }

        async Task GenerateAssetBatch(List<string> prompts, string category)
        {
            RecraftAIManager manager = FindObjectOfType<RecraftAIManager>();
            if (manager == null)
            {
                GameObject obj = new GameObject("RecraftAIManager_Goonzu");
                manager = obj.AddComponent<RecraftAIManager>();
            }

            foreach (string prompt in prompts)
            {
                if (!isGenerating) break;

                string fileName = manager.GenerateFileNameFromPrompt(prompt);
                string filePath = $"Assets/GoonzuGame/{category}/{fileName}";

                manager.GenerateAsset(prompt, filePath);

                // Brief delay between generations
                await Task.Delay(500);
            }
        }

        async Task Generate3DAssetBatch(List<string> prompts, string category)
        {
            MeshyAIManager manager = FindObjectOfType<MeshyAIManager>();
            if (manager == null)
            {
                GameObject obj = new GameObject("MeshyAIManager_Goonzu");
                manager = obj.AddComponent<MeshyAIManager>();
            }

            foreach (string prompt in prompts)
            {
                if (!isGenerating) break;

                Debug.Log($"Generating 3D asset: {prompt}");

                // Simulate 3D generation (would call Meshy AI API)
                string fileName = Generate3DFileNameFromPrompt(prompt);
                string filePath = $"Assets/GoonzuGame/{category}/{fileName}";

                CreatePlaceholder3DModel(filePath);

                // Brief delay between generations
                await Task.Delay(1000);
            }
        }

        string Generate3DFileNameFromPrompt(string prompt)
        {
            string[] keywords = prompt.Split(new char[] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            string fileName = "";

            foreach (string keyword in keywords)
            {
                string cleanKeyword = keyword.ToLower().Replace("-", "_");
                if (cleanKeyword.Length > 2 && !cleanKeyword.Contains("model") && !cleanKeyword.Contains("3d") &&
                    !cleanKeyword.Contains("style") && !cleanKeyword.Contains("fantasy"))
                {
                    fileName += cleanKeyword + "_";
                    if (fileName.Length > 15) break;
                }
            }

            return fileName.TrimEnd('_') + ".fbx";
        }

        void CreatePlaceholder3DModel(string path)
        {
            // Ensure directory exists
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Create a simple placeholder file (in real implementation, this would be downloaded FBX)
            string placeholderContent = $"# Placeholder for 3D model: {Path.GetFileName(path)}\n# This would be a real FBX file downloaded from Meshy AI\n# Prompt used: {path}";
            File.WriteAllText(path.Replace(".fbx", ".txt"), placeholderContent);

            Debug.Log($"Created 3D model placeholder: {path}");
        }
    }
}