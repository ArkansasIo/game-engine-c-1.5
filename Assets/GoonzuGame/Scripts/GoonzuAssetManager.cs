using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame
{
    [System.Serializable]
    public class GoonzuAsset
    {
        public string assetName;
        public Sprite sprite;
        public AssetType assetType;
        public AssetCategory category;
        public string description;
        public Dictionary<string, object> properties = new Dictionary<string, object>();

        public enum AssetType
        {
            Character,
            Building,
            Weapon,
            Armor,
            Consumable,
            Material,
            Creature,
            UI_Element,
            Effect,
            Furniture,
            Tile
        }

        public enum AssetCategory
        {
            Player,
            NPC,
            Environment,
            Item,
            Interface,
            Special
        }
    }

    public class GoonzuAssetManager : MonoBehaviour
    {
        private static GoonzuAssetManager instance;
        public static GoonzuAssetManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GoonzuAssetManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GoonzuAssetManager");
                        instance = go.AddComponent<GoonzuAssetManager>();
                    }
                }
                return instance;
            }
        }

        private Dictionary<string, GoonzuAsset> assetDatabase = new Dictionary<string, GoonzuAsset>();
        private Dictionary<GoonzuAsset.AssetType, List<GoonzuAsset>> assetsByType = new Dictionary<GoonzuAsset.AssetType, List<GoonzuAsset>>();
        private Dictionary<GoonzuAsset.AssetCategory, List<GoonzuAsset>> assetsByCategory = new Dictionary<GoonzuAsset.AssetCategory, List<GoonzuAsset>>();

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAssetDatabase();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void InitializeAssetDatabase()
        {
            // Load all assets from Resources/GoonzuGame folder
            Sprite[] sprites = Resources.LoadAll<Sprite>("GoonzuGame");

            foreach (Sprite sprite in sprites)
            {
                GoonzuAsset asset = CreateAssetFromSprite(sprite);
                if (asset != null)
                {
                    RegisterAsset(asset);
                }
            }

            Debug.Log($"Loaded {assetDatabase.Count} Goonzu assets");
        }

        GoonzuAsset CreateAssetFromSprite(Sprite sprite)
        {
            string assetName = sprite.name;
            GoonzuAsset asset = new GoonzuAsset
            {
                assetName = assetName,
                sprite = sprite
            };

            // Determine asset type from filename
            if (assetName.Contains("warrior") || assetName.Contains("mage") || assetName.Contains("rogue") ||
                assetName.Contains("cleric") || assetName.Contains("paladin") || assetName.Contains("ranger") ||
                assetName.Contains("bard") || assetName.Contains("necromancer") || assetName.Contains("druid") ||
                assetName.Contains("monk") || assetName.Contains("merchant") || assetName.Contains("blacksmith") ||
                assetName.Contains("guard") || assetName.Contains("priest"))
            {
                asset.assetType = GoonzuAsset.AssetType.Character;
                asset.category = assetName.Contains("merchant") || assetName.Contains("blacksmith") ||
                               assetName.Contains("guard") || assetName.Contains("priest") ?
                               GoonzuAsset.AssetCategory.NPC : GoonzuAsset.AssetCategory.Player;
            }
            else if (assetName.Contains("sword") || assetName.Contains("axe") || assetName.Contains("bow") ||
                     assetName.Contains("staff") || assetName.Contains("dagger") || assetName.Contains("mace") ||
                     assetName.Contains("hammer") || assetName.Contains("spear") || assetName.Contains("whip"))
            {
                asset.assetType = GoonzuAsset.AssetType.Weapon;
                asset.category = GoonzuAsset.AssetCategory.Item;
            }
            else if (assetName.Contains("helmet") || assetName.Contains("armor") || assetName.Contains("shield") ||
                     assetName.Contains("boots") || assetName.Contains("gauntlets") || assetName.Contains("robe"))
            {
                asset.assetType = GoonzuAsset.AssetType.Armor;
                asset.category = GoonzuAsset.AssetCategory.Item;
            }
            else if (assetName.Contains("potion") || assetName.Contains("elixir") || assetName.Contains("bread") ||
                     assetName.Contains("gold") || assetName.Contains("crystal") || assetName.Contains("scroll"))
            {
                asset.assetType = GoonzuAsset.AssetType.Consumable;
                asset.category = GoonzuAsset.AssetCategory.Item;
            }
            else if (assetName.Contains("ore") || assetName.Contains("wood") || assetName.Contains("leather") ||
                     assetName.Contains("cloth") || assetName.Contains("gem") || assetName.Contains("bone"))
            {
                asset.assetType = GoonzuAsset.AssetType.Material;
                asset.category = GoonzuAsset.AssetCategory.Item;
            }
            else if (assetName.Contains("dragon") || assetName.Contains("horse") || assetName.Contains("goblin") ||
                     assetName.Contains("troll") || assetName.Contains("skeleton") || assetName.Contains("vampire") ||
                     assetName.Contains("werewolf") || assetName.Contains("phoenix") || assetName.Contains("griffin"))
            {
                asset.assetType = GoonzuAsset.AssetType.Creature;
                asset.category = GoonzuAsset.AssetCategory.Environment;
            }
            else if (assetName.Contains("house") || assetName.Contains("castle") || assetName.Contains("shop") ||
                     assetName.Contains("tavern") || assetName.Contains("temple") || assetName.Contains("tower"))
            {
                asset.assetType = GoonzuAsset.AssetType.Building;
                asset.category = GoonzuAsset.AssetCategory.Environment;
            }
            else if (assetName.Contains("icon") || assetName.Contains("panel") || assetName.Contains("bar"))
            {
                asset.assetType = GoonzuAsset.AssetType.UI_Element;
                asset.category = GoonzuAsset.AssetCategory.Interface;
            }
            else if (assetName.Contains("spell") || assetName.Contains("effect") || assetName.Contains("aura") ||
                     assetName.Contains("explosion") || assetName.Contains("burst"))
            {
                asset.assetType = GoonzuAsset.AssetType.Effect;
                asset.category = GoonzuAsset.AssetCategory.Special;
            }
            else if (assetName.Contains("chair") || assetName.Contains("table") || assetName.Contains("bed") ||
                     assetName.Contains("chest") || assetName.Contains("throne") || assetName.Contains("anvil"))
            {
                asset.assetType = GoonzuAsset.AssetType.Furniture;
                asset.category = GoonzuAsset.AssetCategory.Environment;
            }
            else if (assetName.Contains("tile") || assetName.Contains("floor") || assetName.Contains("ground") ||
                     assetName.Contains("stone") || assetName.Contains("grass") || assetName.Contains("dirt"))
            {
                asset.assetType = GoonzuAsset.AssetType.Tile;
                asset.category = GoonzuAsset.AssetCategory.Environment;
            }

            asset.description = GenerateDescription(asset);
            InitializeAssetProperties(asset);

            return asset;
        }

        void RegisterAsset(GoonzuAsset asset)
        {
            assetDatabase[asset.assetName] = asset;

            if (!assetsByType.ContainsKey(asset.assetType))
                assetsByType[asset.assetType] = new List<GoonzuAsset>();
            assetsByType[asset.assetType].Add(asset);

            if (!assetsByCategory.ContainsKey(asset.category))
                assetsByCategory[asset.category] = new List<GoonzuAsset>();
            assetsByCategory[asset.category].Add(asset);
        }

        string GenerateDescription(GoonzuAsset asset)
        {
            // Generate contextual descriptions based on asset type
            switch (asset.assetType)
            {
                case GoonzuAsset.AssetType.Character:
                    return $"A medieval fantasy {asset.assetName.Replace("_", " ")} character";
                case GoonzuAsset.AssetType.Weapon:
                    return $"A powerful {asset.assetName.Replace("_", " ")} weapon";
                case GoonzuAsset.AssetType.Armor:
                    return $"Protective {asset.assetName.Replace("_", " ")} armor";
                case GoonzuAsset.AssetType.Building:
                    return $"A grand {asset.assetName.Replace("_", " ")} structure";
                case GoonzuAsset.AssetType.Creature:
                    return $"A formidable {asset.assetName.Replace("_", " ")} creature";
                default:
                    return $"A {asset.assetName.Replace("_", " ")} asset";
            }
        }

        void InitializeAssetProperties(GoonzuAsset asset)
        {
            // Initialize default properties based on asset type
            switch (asset.assetType)
            {
                case GoonzuAsset.AssetType.Weapon:
                    asset.properties["damage"] = 10;
                    asset.properties["durability"] = 100;
                    asset.properties["rarity"] = "common";
                    break;
                case GoonzuAsset.AssetType.Armor:
                    asset.properties["defense"] = 5;
                    asset.properties["durability"] = 100;
                    asset.properties["rarity"] = "common";
                    break;
                case GoonzuAsset.AssetType.Consumable:
                    asset.properties["effect"] = "heal";
                    asset.properties["power"] = 20;
                    asset.properties["stackable"] = true;
                    break;
                case GoonzuAsset.AssetType.Character:
                    asset.properties["level"] = 1;
                    asset.properties["health"] = 100;
                    asset.properties["mana"] = 50;
                    break;
            }
        }

        // Public API methods
        public GoonzuAsset GetAsset(string assetName)
        {
            return assetDatabase.ContainsKey(assetName) ? assetDatabase[assetName] : null;
        }

        public List<GoonzuAsset> GetAssetsByType(GoonzuAsset.AssetType type)
        {
            return assetsByType.ContainsKey(type) ? assetsByType[type] : new List<GoonzuAsset>();
        }

        public List<GoonzuAsset> GetAssetsByCategory(GoonzuAsset.AssetCategory category)
        {
            return assetsByCategory.ContainsKey(category) ? assetsByCategory[category] : new List<GoonzuAsset>();
        }

        public Sprite GetSprite(string assetName)
        {
            GoonzuAsset asset = GetAsset(assetName);
            return asset != null ? asset.sprite : null;
        }

        public List<string> GetAllAssetNames()
        {
            return new List<string>(assetDatabase.Keys);
        }

        public int GetAssetCount()
        {
            return assetDatabase.Count;
        }
    }
}