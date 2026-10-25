using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace AIAssetTools
{
    /// <summary>
    /// Editor tool for generating placeholder asset files with prompts
    /// </summary>
    public class AssetPlaceholderGenerator : EditorWindow
    {
        private Dictionary<string, string> assetPrompts = new Dictionary<string, string>()
        {
            // Characters
            {"Characters/player_male", "male fantasy adventurer, light armor, standing pose, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"},
            {"Characters/player_female", "female fantasy adventurer, leather armor, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"},
            {"Characters/npc_merchant", "friendly town merchant NPC, medieval clothing, market trader, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"},
            {"Characters/npc_blacksmith", "blacksmith NPC holding hammer, apron, medieval workshop worker, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"},

            // Buildings
            {"Buildings/house_01", "isometric medieval town house, timber frame architecture, bright fantasy colors, anime-inspired fantasy MMORPG style, game asset"},
            {"Buildings/blacksmith_shop", "fantasy blacksmith shop building, stone walls, forge chimney, medieval town building, anime-inspired fantasy MMORPG style, game asset"},
            {"Buildings/market_stall", "isometric market stall with colorful canopy, fantasy RPG town asset, anime-inspired fantasy MMORPG style, game asset"},

            // Tiles
            {"Tiles/grass_tile", "seamless grass tile texture, top-down, hand-painted fantasy style, RPG map tileset, anime-inspired fantasy MMORPG style"},
            {"Tiles/dirt_path", "dirt road tile seamless texture, anime-inspired fantasy MMORPG style"},
            {"Tiles/stone_plaza", "stone plaza tile medieval town, seamless, anime-inspired fantasy MMORPG style"},

            // Items
            {"Items/Weapons/sword", "fantasy sword inventory icon, game inventory item, bright colors, clean outline, transparent background, anime-inspired fantasy MMORPG style"},
            {"Items/Armor/helmet", "iron helmet armor icon, RPG equipment item, anime-inspired fantasy MMORPG style"},
            {"Items/Materials/iron_ore", "iron ore resource icon, anime-inspired fantasy MMORPG style"},
            {"Items/Consumables/health_potion", "red health potion bottle icon, anime-inspired fantasy MMORPG style"},

            // Creatures
            {"Creatures/Mounts/horse", "cute fantasy horse mount side view, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"},
            {"Creatures/Monsters/slime", "small slime monster enemy, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"},

            // UI
            {"UI/Panels/inventory_panel", "fantasy MMORPG inventory window frame, ornate medieval border, gold trim, transparent background, game UI panel, anime-inspired fantasy MMORPG style"},
            {"UI/Icons/gold_coin", "gold coin currency icon, clean simple symbol, bright colors, transparent background, anime-inspired fantasy MMORPG style"},
            {"UI/Bars/health_bar", "fantasy health bar UI, RPG HUD element, clean fantasy style, anime-inspired fantasy MMORPG style"},
            {"UI/Windows/trade_window", "fantasy MMORPG trading interface panel, marketplace UI frame, ornate border, anime-inspired fantasy MMORPG style"}
        };

        [MenuItem("Tools/AI Asset Tools/Generate Placeholders")]
        static void ShowWindow()
        {
            GetWindow<AssetPlaceholderGenerator>("Asset Placeholder Generator");
        }

        void OnGUI()
        {
            GUILayout.Label("Generate Asset Placeholders", EditorStyles.boldLabel);
            GUILayout.Label("This will create .txt placeholder files with AI prompts for each asset type.");

            if (GUILayout.Button("Generate All Placeholders"))
            {
                GenerateAllPlaceholders();
            }

            if (GUILayout.Button("Clear All Placeholders"))
            {
                ClearAllPlaceholders();
            }
        }

        void GenerateAllPlaceholders()
        {
            foreach (var kvp in assetPrompts)
            {
                string filePath = $"Assets/GoonzuGame/{kvp.Key}.txt";
                string directory = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string content = $"# Placeholder for {Path.GetFileName(kvp.Key)}.png\n# Generate using: {kvp.Value}";
                File.WriteAllText(filePath, content);
            }

            AssetDatabase.Refresh();
            Debug.Log("Generated all asset placeholders!");
        }

        void ClearAllPlaceholders()
        {
            foreach (var kvp in assetPrompts)
            {
                string filePath = $"Assets/GoonzuGame/{kvp.Key}.txt";
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("Cleared all asset placeholders!");
        }
    }
}