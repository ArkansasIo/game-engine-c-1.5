using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame
{
    [System.Serializable]
    public class GoonzuItem
    {
        public string itemName;
        public Sprite icon;
        public ItemType itemType;
        public ItemRarity rarity;
        public int level = 1;
        public int value = 0;
        public bool stackable = false;
        public int maxStack = 1;
        public int currentStack = 1;

        public Dictionary<string, object> properties = new Dictionary<string, object>();

        public enum ItemType
        {
            Weapon,
            Armor,
            Consumable,
            Material,
            Quest,
            Miscellaneous
        }

        public enum ItemRarity
        {
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary,
            Mythical
        }

        public GoonzuItem(string name, ItemType type, ItemRarity itemRarity = ItemRarity.Common)
        {
            itemName = name;
            itemType = type;
            rarity = itemRarity;
            InitializeProperties();
        }

        void InitializeProperties()
        {
            switch (itemType)
            {
                case ItemType.Weapon:
                    properties["damage"] = 10;
                    properties["durability"] = 100;
                    properties["attackSpeed"] = 1.0f;
                    stackable = false;
                    break;
                case ItemType.Armor:
                    properties["defense"] = 5;
                    properties["durability"] = 100;
                    properties["movementPenalty"] = 0f;
                    stackable = false;
                    break;
                case ItemType.Consumable:
                    properties["effect"] = "heal";
                    properties["power"] = 20;
                    properties["cooldown"] = 0f;
                    stackable = true;
                    maxStack = 99;
                    break;
                case ItemType.Material:
                    properties["quality"] = "normal";
                    properties["weight"] = 1f;
                    stackable = true;
                    maxStack = 999;
                    break;
            }

            // Set value based on rarity and level
            value = CalculateValue();
        }

        int CalculateValue()
        {
            int baseValue = level * 10;
            float rarityMultiplier = 1f;

            switch (rarity)
            {
                case ItemRarity.Common: rarityMultiplier = 1f; break;
                case ItemRarity.Uncommon: rarityMultiplier = 2f; break;
                case ItemRarity.Rare: rarityMultiplier = 5f; break;
                case ItemRarity.Epic: rarityMultiplier = 10f; break;
                case ItemRarity.Legendary: rarityMultiplier = 25f; break;
                case ItemRarity.Mythical: rarityMultiplier = 50f; break;
            }

            return Mathf.RoundToInt(baseValue * rarityMultiplier);
        }

        public object GetProperty(string key, object defaultValue = null)
        {
            return properties.ContainsKey(key) ? properties[key] : defaultValue;
        }

        public void SetProperty(string key, object value)
        {
            properties[key] = value;
        }

        public Color GetRarityColor()
        {
            switch (rarity)
            {
                case ItemRarity.Common: return Color.white;
                case ItemRarity.Uncommon: return Color.green;
                case ItemRarity.Rare: return Color.blue;
                case ItemRarity.Epic: return new Color(0.7f, 0f, 1f); // Purple
                case ItemRarity.Legendary: return new Color(1f, 0.5f, 0f); // Orange
                case ItemRarity.Mythical: return Color.red;
                default: return Color.white;
            }
        }

        public string GetDescription()
        {
            string desc = $"{rarity} {itemType}";

            switch (itemType)
            {
                case ItemType.Weapon:
                    int damage = (int)GetProperty("damage", 0);
                    desc += $"\nDamage: {damage}";
                    break;
                case ItemType.Armor:
                    int defense = (int)GetProperty("defense", 0);
                    desc += $"\nDefense: {defense}";
                    break;
                case ItemType.Consumable:
                    string effect = (string)GetProperty("effect", "unknown");
                    int power = (int)GetProperty("power", 0);
                    desc += $"\n{effect.ToUpper()}: {power}";
                    break;
            }

            desc += $"\nValue: {value} gold";
            if (level > 1) desc += $"\nRequired Level: {level}";

            return desc;
        }
    }

    public class GoonzuItemManager : MonoBehaviour
    {
        private static GoonzuItemManager instance;
        public static GoonzuItemManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GoonzuItemManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GoonzuItemManager");
                        instance = go.AddComponent<GoonzuItemManager>();
                    }
                }
                return instance;
            }
        }

        private Dictionary<string, GoonzuItem> itemDatabase = new Dictionary<string, GoonzuItem>();

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeItemDatabase();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void InitializeItemDatabase()
        {
            // Create all weapons
            CreateWeapon("broadsword", 15, GoonzuItem.ItemRarity.Common);
            CreateWeapon("longsword", 18, GoonzuItem.ItemRarity.Uncommon);
            CreateWeapon("battleaxe", 22, GoonzuItem.ItemRarity.Uncommon);
            CreateWeapon("warhammer", 25, GoonzuItem.ItemRarity.Rare);
            CreateWeapon("dagger", 8, GoonzuItem.ItemRarity.Common);
            CreateWeapon("bow", 12, GoonzuItem.ItemRarity.Common);
            CreateWeapon("crossbow", 20, GoonzuItem.ItemRarity.Uncommon);
            CreateWeapon("staff", 10, GoonzuItem.ItemRarity.Common, 1.2f);
            CreateWeapon("wand", 8, GoonzuItem.ItemRarity.Common, 1.5f);
            CreateWeapon("spear", 16, GoonzuItem.ItemRarity.Common);
            CreateWeapon("halberd", 24, GoonzuItem.ItemRarity.Rare);
            CreateWeapon("scimitar", 17, GoonzuItem.ItemRarity.Uncommon);
            CreateWeapon("rapier", 14, GoonzuItem.ItemRarity.Uncommon);
            CreateWeapon("claymore", 28, GoonzuItem.ItemRarity.Epic);
            CreateWeapon("flail", 20, GoonzuItem.ItemRarity.Uncommon);
            CreateWeapon("morningstar", 23, GoonzuItem.ItemRarity.Rare);
            CreateWeapon("quarterstaff", 12, GoonzuItem.ItemRarity.Common, 1.1f);
            CreateWeapon("shortbow", 10, GoonzuItem.ItemRarity.Common);
            CreateWeapon("longbow", 16, GoonzuItem.ItemRarity.Uncommon);
            CreateWeapon("throwing_knife", 6, GoonzuItem.ItemRarity.Common);
            CreateWeapon("sling", 4, GoonzuItem.ItemRarity.Common);
            CreateWeapon("whip", 9, GoonzuItem.ItemRarity.Uncommon);
            CreateWeapon("katana", 21, GoonzuItem.ItemRarity.Rare);
            CreateWeapon("scythe", 19, GoonzuItem.ItemRarity.Uncommon);

            // Create all armor
            CreateArmor("plate_helmet", 8, GoonzuItem.ItemRarity.Uncommon);
            CreateArmor("chainmail_helmet", 5, GoonzuItem.ItemRarity.Common);
            CreateArmor("leather_helmet", 3, GoonzuItem.ItemRarity.Common);
            CreateArmor("wizard_hat", 2, GoonzuItem.ItemRarity.Common);
            CreateArmor("knight_armor", 15, GoonzuItem.ItemRarity.Rare);
            CreateArmor("chainmail_armor", 10, GoonzuItem.ItemRarity.Uncommon);
            CreateArmor("leather_armor", 6, GoonzuItem.ItemRarity.Common);
            CreateArmor("robe", 4, GoonzuItem.ItemRarity.Common);
            CreateArmor("cloak", 3, GoonzuItem.ItemRarity.Common);
            CreateArmor("shield", 7, GoonzuItem.ItemRarity.Common);
            CreateArmor("buckler", 4, GoonzuItem.ItemRarity.Common);
            CreateArmor("tower_shield", 12, GoonzuItem.ItemRarity.Rare);
            CreateArmor("gauntlets", 3, GoonzuItem.ItemRarity.Common);
            CreateArmor("boots", 3, GoonzuItem.ItemRarity.Common);
            CreateArmor("belt", 2, GoonzuItem.ItemRarity.Common);
            CreateArmor("pauldrons", 4, GoonzuItem.ItemRarity.Uncommon);
            CreateArmor("vambraces", 3, GoonzuItem.ItemRarity.Common);
            CreateArmor("greaves", 4, GoonzuItem.ItemRarity.Uncommon);
            CreateArmor("cuirass", 8, GoonzuItem.ItemRarity.Uncommon);
            CreateArmor("surcoat", 1, GoonzuItem.ItemRarity.Common);

            // Create all consumables
            CreateConsumable("health_potion", "heal", 50, GoonzuItem.ItemRarity.Common);
            CreateConsumable("mana_potion", "restore_mana", 30, GoonzuItem.ItemRarity.Common);
            CreateConsumable("strength_potion", "buff_strength", 10, GoonzuItem.ItemRarity.Uncommon);
            CreateConsumable("agility_potion", "buff_dexterity", 10, GoonzuItem.ItemRarity.Uncommon);
            CreateConsumable("intelligence_potion", "buff_intelligence", 10, GoonzuItem.ItemRarity.Uncommon);
            CreateConsumable("bread", "heal", 10, GoonzuItem.ItemRarity.Common);
            CreateConsumable("cheese", "heal", 15, GoonzuItem.ItemRarity.Common);
            CreateConsumable("apple", "heal", 8, GoonzuItem.ItemRarity.Common);
            CreateConsumable("wine", "restore_mana", 20, GoonzuItem.ItemRarity.Common);
            CreateConsumable("ale", "heal", 12, GoonzuItem.ItemRarity.Common);
            CreateConsumable("herb_bundle", "heal", 25, GoonzuItem.ItemRarity.Uncommon);
            CreateConsumable("magic_crystal", "restore_mana", 40, GoonzuItem.ItemRarity.Uncommon);
            CreateConsumable("scroll", "random_buff", 0, GoonzuItem.ItemRarity.Rare);
            CreateConsumable("rune_stone", "teleport", 0, GoonzuItem.ItemRarity.Rare);
            CreateConsumable("elixir", "full_restore", 100, GoonzuItem.ItemRarity.Epic);

            // Create all materials
            CreateMaterial("iron_ore", GoonzuItem.ItemRarity.Common);
            CreateMaterial("gold_ore", GoonzuItem.ItemRarity.Uncommon);
            CreateMaterial("mithril_ore", GoonzuItem.ItemRarity.Rare);
            CreateMaterial("adamantite_ore", GoonzuItem.ItemRarity.Epic);
            CreateMaterial("wood_log", GoonzuItem.ItemRarity.Common);
            CreateMaterial("leather_hide", GoonzuItem.ItemRarity.Common);
            CreateMaterial("cloth_bolt", GoonzuItem.ItemRarity.Common);
            CreateMaterial("gemstone", GoonzuItem.ItemRarity.Rare);
            CreateMaterial("bone", GoonzuItem.ItemRarity.Common);
            CreateMaterial("crystal", GoonzuItem.ItemRarity.Uncommon);

            Debug.Log($"Initialized {itemDatabase.Count} items in database");
        }

        void CreateWeapon(string name, int damage, GoonzuItem.ItemRarity rarity, float attackSpeed = 1.0f)
        {
            GoonzuItem weapon = new GoonzuItem(name, GoonzuItem.ItemType.Weapon, rarity);
            weapon.SetProperty("damage", damage);
            weapon.SetProperty("attackSpeed", attackSpeed);
            weapon.icon = GoonzuAssetManager.Instance.GetSprite($"Weapons/{name}");
            itemDatabase[name] = weapon;
        }

        void CreateArmor(string name, int defense, GoonzuItem.ItemRarity rarity)
        {
            GoonzuItem armor = new GoonzuItem(name, GoonzuItem.ItemType.Armor, rarity);
            armor.SetProperty("defense", defense);
            armor.icon = GoonzuAssetManager.Instance.GetSprite($"Armor/{name}");
            itemDatabase[name] = armor;
        }

        void CreateConsumable(string name, string effect, int power, GoonzuItem.ItemRarity rarity)
        {
            GoonzuItem consumable = new GoonzuItem(name, GoonzuItem.ItemType.Consumable, rarity);
            consumable.SetProperty("effect", effect);
            consumable.SetProperty("power", power);
            consumable.icon = GoonzuAssetManager.Instance.GetSprite($"Consumables/{name}");
            itemDatabase[name] = consumable;
        }

        void CreateMaterial(string name, GoonzuItem.ItemRarity rarity)
        {
            GoonzuItem material = new GoonzuItem(name, GoonzuItem.ItemType.Material, rarity);
            material.icon = GoonzuAssetManager.Instance.GetSprite($"Materials/{name}");
            itemDatabase[name] = material;
        }

        // Public API methods
        public GoonzuItem GetItem(string itemName)
        {
            return itemDatabase.ContainsKey(itemName) ? itemDatabase[itemName] : null;
        }

        public GoonzuItem CreateItem(string itemName, int level = 1, GoonzuItem.ItemRarity rarity = GoonzuItem.ItemRarity.Common)
        {
            GoonzuItem baseItem = GetItem(itemName);
            if (baseItem == null) return null;

            GoonzuItem newItem = new GoonzuItem(baseItem.itemName, baseItem.itemType, rarity);
            newItem.level = level;

            // Scale properties based on level and rarity
            float levelMultiplier = 1f + (level - 1) * 0.1f;
            float rarityMultiplier = 1f;

            switch (rarity)
            {
                case GoonzuItem.ItemRarity.Uncommon: rarityMultiplier = 1.2f; break;
                case GoonzuItem.ItemRarity.Rare: rarityMultiplier = 1.5f; break;
                case GoonzuItem.ItemRarity.Epic: rarityMultiplier = 2f; break;
                case GoonzuItem.ItemRarity.Legendary: rarityMultiplier = 3f; break;
                case GoonzuItem.ItemRarity.Mythical: rarityMultiplier = 5f; break;
            }

            float totalMultiplier = levelMultiplier * rarityMultiplier;

            if (newItem.itemType == GoonzuItem.ItemType.Weapon)
            {
                int baseDamage = (int)baseItem.GetProperty("damage", 10);
                newItem.SetProperty("damage", Mathf.RoundToInt(baseDamage * totalMultiplier));
            }
            else if (newItem.itemType == GoonzuItem.ItemType.Armor)
            {
                int baseDefense = (int)baseItem.GetProperty("defense", 5);
                newItem.SetProperty("defense", Mathf.RoundToInt(baseDefense * totalMultiplier));
            }

            return newItem;
        }

        public List<GoonzuItem> GetItemsByType(GoonzuItem.ItemType type)
        {
            List<GoonzuItem> items = new List<GoonzuItem>();
            foreach (var item in itemDatabase.Values)
            {
                if (item.itemType == type)
                {
                    items.Add(item);
                }
            }
            return items;
        }

        public List<GoonzuItem> GetItemsByRarity(GoonzuItem.ItemRarity rarity)
        {
            List<GoonzuItem> items = new List<GoonzuItem>();
            foreach (var item in itemDatabase.Values)
            {
                if (item.rarity == rarity)
                {
                    items.Add(item);
                }
            }
            return items;
        }

        public GoonzuItem GenerateRandomItem(int level, GoonzuItem.ItemType? typeFilter = null)
        {
            List<GoonzuItem> candidates = new List<GoonzuItem>();

            foreach (var item in itemDatabase.Values)
            {
                if (typeFilter == null || item.itemType == typeFilter)
                {
                    candidates.Add(item);
                }
            }

            if (candidates.Count == 0) return null;

            GoonzuItem baseItem = candidates[Random.Range(0, candidates.Count)];

            // Random rarity based on level
            GoonzuItem.ItemRarity rarity = GoonzuItem.ItemRarity.Common;
            float rand = Random.value;

            if (level >= 5 && rand < 0.3f) rarity = GoonzuItem.ItemRarity.Uncommon;
            if (level >= 10 && rand < 0.15f) rarity = GoonzuItem.ItemRarity.Rare;
            if (level >= 15 && rand < 0.05f) rarity = GoonzuItem.ItemRarity.Epic;
            if (level >= 20 && rand < 0.01f) rarity = GoonzuItem.ItemRarity.Legendary;

            return CreateItem(baseItem.itemName, level, rarity);
        }
    }
}