using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame
{
    public class GoonzuItemManager : MonoBehaviour
    {
        public static GoonzuItemManager Instance { get; private set; }

        [Header("Item Generation")]
        public int maxItemsInDatabase = 1000;
        public Dictionary<string, GoonzuItem> itemDatabase = new Dictionary<string, GoonzuItem>();

        [Header("Item Generation Settings")]
        public List<string> weaponPrefixes = new List<string>();
        public List<string> weaponSuffixes = new List<string>();
        public List<string> armorPrefixes = new List<string>();
        public List<string> armorSuffixes = new List<string>();
        public List<string> accessoryPrefixes = new List<string>();
        public List<string> accessorySuffixes = new List<string>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            InitializeItemPrefixes();
            GenerateItemDatabase();
        }

        void InitializeItemPrefixes()
        {
            // Weapon prefixes
            weaponPrefixes.AddRange(new string[] {
                "Sharp", "Deadly", "Mighty", "Ancient", "Cursed", "Blessed",
                "Fiery", "Icy", "Storm", "Venomous", "Sacred", "Profane",
                "Elven", "Dwarven", "Orcish", "Dragon", "Phoenix", "Griffin"
            });

            // Weapon suffixes
            weaponSuffixes.AddRange(new string[] {
                "of Power", "of Speed", "of Accuracy", "of the Wolf", "of the Bear",
                "of the Eagle", "of Flame", "of Frost", "of Thunder", "of Poison",
                "of Light", "of Darkness", "of the Ancients", "of the Dragon",
                "of the Phoenix", "of Victory", "of Slaughter", "of Precision"
            });

            // Armor prefixes
            armorPrefixes.AddRange(new string[] {
                "Reinforced", "Enchanted", "Mystic", "Ancient", "Cursed", "Blessed",
                "Fiery", "Icy", "Storm", "Venomous", "Sacred", "Profane",
                "Elven", "Dwarven", "Orcish", "Dragon", "Phoenix", "Griffin"
            });

            // Armor suffixes
            armorSuffixes.AddRange(new string[] {
                "of Protection", "of Defense", "of Resilience", "of the Wolf", "of the Bear",
                "of the Eagle", "of Flame", "of Frost", "of Thunder", "of Poison",
                "of Light", "of Darkness", "of the Ancients", "of the Dragon",
                "of the Phoenix", "of Guardians", "of Fortitude", "of Resistance"
            });

            // Accessory prefixes
            accessoryPrefixes.AddRange(new string[] {
                "Lucky", "Enchanted", "Mystic", "Ancient", "Cursed", "Blessed",
                "Fiery", "Icy", "Storm", "Venomous", "Sacred", "Profane",
                "Elven", "Dwarven", "Orcish", "Dragon", "Phoenix", "Griffin"
            });

            // Accessory suffixes
            accessorySuffixes.AddRange(new string[] {
                "of Fortune", "of Wisdom", "of Power", "of the Wolf", "of the Bear",
                "of the Eagle", "of Flame", "of Frost", "of Thunder", "of Poison",
                "of Light", "of Darkness", "of the Ancients", "of the Dragon",
                "of the Phoenix", "of Prosperity", "of Knowledge", "of Might"
            });
        }

        void GenerateItemDatabase()
        {
            // Generate weapons
            GenerateWeapons();

            // Generate armor
            GenerateArmor();

            // Generate accessories
            GenerateAccessories();

            // Generate consumables
            GenerateConsumables();

            // Generate materials
            GenerateMaterials();

            Debug.Log($"Generated {itemDatabase.Count} items in database");
        }

        void GenerateWeapons()
        {
            string[] weaponTypes = { "Sword", "Axe", "Mace", "Dagger", "Bow", "Staff", "Wand", "Hammer" };
            string[] weaponMaterials = { "Iron", "Steel", "Silver", "Gold", "Mithril", "Adamant", "Crystal", "Shadow" };

            int itemCount = 0;
            foreach (string type in weaponTypes)
            {
                foreach (string material in weaponMaterials)
                {
                    if (itemCount >= maxItemsInDatabase / 5) break; // Limit weapons to 1/5 of database

                    GoonzuItem weapon = CreateWeapon(type, material);
                    itemDatabase[weapon.itemID] = weapon;
                    itemCount++;
                }
            }
        }

        void GenerateArmor()
        {
            string[] armorTypes = { "Helmet", "Chestplate", "Leggings", "Boots", "Gloves", "Shield" };
            string[] armorMaterials = { "Leather", "Chain", "Plate", "Scale", "Bone", "Dragonscale", "Mithril", "Shadow" };

            int itemCount = 0;
            foreach (string type in armorTypes)
            {
                foreach (string material in armorMaterials)
                {
                    if (itemCount >= maxItemsInDatabase / 5) break; // Limit armor to 1/5 of database

                    GoonzuItem armor = CreateArmor(type, material);
                    itemDatabase[armor.itemID] = armor;
                    itemCount++;
                }
            }
        }

        void GenerateAccessories()
        {
            string[] accessoryTypes = { "Ring", "Amulet", "Necklace", "Bracelet", "Earring", "Talisman" };
            string[] accessoryMaterials = { "Silver", "Gold", "Platinum", "Crystal", "Bone", "Gem", "Pearl", "Shadow" };

            int itemCount = 0;
            foreach (string type in accessoryTypes)
            {
                foreach (string material in accessoryMaterials)
                {
                    if (itemCount >= maxItemsInDatabase / 5) break; // Limit accessories to 1/5 of database

                    GoonzuItem accessory = CreateAccessory(type, material);
                    itemDatabase[accessory.itemID] = accessory;
                    itemCount++;
                }
            }
        }

        void GenerateConsumables()
        {
            string[] potionTypes = { "Health", "Mana", "Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma" };
            string[] potionSizes = { "Small", "Medium", "Large", "Greater", "Supreme" };

            int itemCount = 0;
            foreach (string type in potionTypes)
            {
                foreach (string size in potionSizes)
                {
                    if (itemCount >= maxItemsInDatabase / 5) break; // Limit consumables to 1/5 of database

                    GoonzuItem potion = CreatePotion(type, size);
                    itemDatabase[potion.itemID] = potion;
                    itemCount++;
                }
            }

            // Add some food items
            string[] foodTypes = { "Bread", "Cheese", "Meat", "Fruit", "Vegetable", "Fish" };
            foreach (string food in foodTypes)
            {
                GoonzuItem foodItem = CreateFood(food);
                itemDatabase[foodItem.itemID] = foodItem;
            }
        }

        void GenerateMaterials()
        {
            string[] materials = {
                "Iron Ore", "Copper Ore", "Silver Ore", "Gold Ore", "Mithril Ore", "Adamant Ore",
                "Leather", "Wolf Hide", "Bear Hide", "Dragon Scale", "Phoenix Feather", "Griffin Claw",
                "Crystal", "Gem", "Pearl", "Bone", "Wood", "Herb", "Mushroom", "Flower"
            };

            foreach (string material in materials)
            {
                GoonzuItem materialItem = CreateMaterial(material);
                itemDatabase[materialItem.itemID] = materialItem;
            }
        }

        GoonzuItem CreateWeapon(string type, string material)
        {
            GoonzuItem weapon = new GoonzuItem();
            weapon.itemType = GoonzuItem.ItemType.Weapon;
            weapon.itemName = $"{material} {type}";
            weapon.itemID = $"weapon_{material.ToLower()}_{type.ToLower()}";
            weapon.description = $"A {material.ToLower()} {type.ToLower()} forged by skilled craftsmen.";
            weapon.stackCount = 1;
            weapon.maxStack = 1;

            // Set rarity based on material
            weapon.rarity = GetMaterialRarity(material);

            // Set base stats
            weapon.damage = GetWeaponBaseDamage(type) + GetMaterialDamageBonus(material);
            weapon.armor = 0;
            weapon.strengthBonus = Random.Range(0, 3);
            weapon.dexterityBonus = type == "Bow" || type == "Dagger" ? Random.Range(1, 4) : 0;
            weapon.intelligenceBonus = type == "Staff" || type == "Wand" ? Random.Range(1, 4) : 0;

            // Set value
            weapon.value = CalculateItemValue(weapon);

            // Load sprite
            weapon.icon = GoonzuAssetManager.Instance.GetSprite($"Weapons/{weapon.itemID}");

            return weapon;
        }

        GoonzuItem CreateArmor(string type, string material)
        {
            GoonzuItem armor = new GoonzuItem();
            armor.itemType = GoonzuItem.ItemType.Armor;
            armor.itemName = $"{material} {type}";
            armor.itemID = $"armor_{material.ToLower()}_{type.ToLower()}";
            armor.description = $"A {material.ToLower()} {type.ToLower()} that provides protection.";
            armor.stackCount = 1;
            armor.maxStack = 1;

            // Set rarity based on material
            armor.rarity = GetMaterialRarity(material);

            // Set base stats
            armor.damage = 0;
            armor.armor = GetArmorBaseDefense(type) + GetMaterialArmorBonus(material);
            armor.constitutionBonus = Random.Range(0, 3);
            armor.strengthBonus = type == "Shield" ? Random.Range(1, 3) : 0;

            // Set value
            armor.value = CalculateItemValue(armor);

            // Load sprite
            armor.icon = GoonzuAssetManager.Instance.GetSprite($"Armor/{armor.itemID}");

            return armor;
        }

        GoonzuItem CreateAccessory(string type, string material)
        {
            GoonzuItem accessory = new GoonzuItem();
            accessory.itemType = GoonzuItem.ItemType.Accessory;
            accessory.itemName = $"{material} {type}";
            accessory.itemID = $"accessory_{material.ToLower()}_{type.ToLower()}";
            accessory.description = $"A {material.ToLower()} {type.ToLower()} with mystical properties.";
            accessory.stackCount = 1;
            accessory.maxStack = 1;

            // Set rarity based on material
            accessory.rarity = GetMaterialRarity(material);

            // Set random stat bonuses
            accessory.strengthBonus = Random.Range(0, 2);
            accessory.dexterityBonus = Random.Range(0, 2);
            accessory.constitutionBonus = Random.Range(0, 2);
            accessory.intelligenceBonus = Random.Range(0, 2);
            accessory.wisdomBonus = Random.Range(0, 2);
            accessory.charismaBonus = Random.Range(0, 2);

            // Set value
            accessory.value = CalculateItemValue(accessory);

            // Load sprite
            accessory.icon = GoonzuAssetManager.Instance.GetSprite($"Accessories/{accessory.itemID}");

            return accessory;
        }

        GoonzuItem CreatePotion(string type, string size)
        {
            GoonzuItem potion = new GoonzuItem();
            potion.itemType = GoonzuItem.ItemType.Consumable;
            potion.itemName = $"{size} {type} Potion";
            potion.itemID = $"potion_{type.ToLower()}_{size.ToLower()}";
            potion.description = $"A {size.ToLower()} potion that restores {type.ToLower()}.";
            potion.stackCount = 1;
            potion.maxStack = 5;

            potion.rarity = GoonzuItem.ItemRarity.Common;
            potion.value = GetPotionValue(size);

            // Load sprite
            potion.icon = GoonzuAssetManager.Instance.GetSprite($"Consumables/{potion.itemID}");

            return potion;
        }

        GoonzuItem CreateFood(string foodType)
        {
            GoonzuItem food = new GoonzuItem();
            food.itemType = GoonzuItem.ItemType.Consumable;
            food.itemName = foodType;
            food.itemID = $"food_{foodType.ToLower()}";
            food.description = $"A piece of {foodType.ToLower()} that restores some health.";
            food.stackCount = 1;
            food.maxStack = 10;

            food.rarity = GoonzuItem.ItemRarity.Common;
            food.value = Random.Range(1, 5);

            // Load sprite
            food.icon = GoonzuAssetManager.Instance.GetSprite($"Consumables/{food.itemID}");

            return food;
        }

        GoonzuItem CreateMaterial(string materialType)
        {
            GoonzuItem material = new GoonzuItem();
            material.itemType = GoonzuItem.ItemType.Material;
            material.itemName = materialType;
            material.itemID = $"material_{materialType.ToLower().Replace(" ", "_")}";
            material.description = $"A piece of {materialType.ToLower()} used in crafting.";
            material.stackCount = 1;
            material.maxStack = 99;

            material.rarity = GoonzuItem.ItemRarity.Common;
            material.value = Random.Range(1, 10);

            // Load sprite
            material.icon = GoonzuAssetManager.Instance.GetSprite($"Materials/{material.itemID}");

            return material;
        }

        GoonzuItem.ItemRarity GetMaterialRarity(string material)
        {
            switch (material.ToLower())
            {
                case "iron": return GoonzuItem.ItemRarity.Common;
                case "steel": return GoonzuItem.ItemRarity.Uncommon;
                case "silver": return GoonzuItem.ItemRarity.Uncommon;
                case "gold": return GoonzuItem.ItemRarity.Rare;
                case "mithril": return GoonzuItem.ItemRarity.Rare;
                case "adamant": return GoonzuItem.ItemRarity.Epic;
                case "crystal": return GoonzuItem.ItemRarity.Epic;
                case "shadow": return GoonzuItem.ItemRarity.Legendary;
                default: return GoonzuItem.ItemRarity.Common;
            }
        }

        int GetWeaponBaseDamage(string weaponType)
        {
            switch (weaponType)
            {
                case "Sword": return 8;
                case "Axe": return 10;
                case "Mace": return 9;
                case "Dagger": return 5;
                case "Bow": return 7;
                case "Staff": return 6;
                case "Wand": return 4;
                case "Hammer": return 11;
                default: return 6;
            }
        }

        int GetMaterialDamageBonus(string material)
        {
            switch (material.ToLower())
            {
                case "iron": return 0;
                case "steel": return 2;
                case "silver": return 1;
                case "gold": return 3;
                case "mithril": return 4;
                case "adamant": return 6;
                case "crystal": return 5;
                case "shadow": return 8;
                default: return 0;
            }
        }

        int GetArmorBaseDefense(string armorType)
        {
            switch (armorType)
            {
                case "Helmet": return 3;
                case "Chestplate": return 8;
                case "Leggings": return 6;
                case "Boots": return 2;
                case "Gloves": return 2;
                case "Shield": return 5;
                default: return 4;
            }
        }

        int GetMaterialArmorBonus(string material)
        {
            switch (material.ToLower())
            {
                case "leather": return 0;
                case "chain": return 2;
                case "plate": return 4;
                case "scale": return 3;
                case "bone": return 1;
                case "dragonscale": return 6;
                case "mithril": return 5;
                case "shadow": return 7;
                default: return 0;
            }
        }

        int GetPotionValue(string size)
        {
            switch (size)
            {
                case "Small": return 10;
                case "Medium": return 25;
                case "Large": return 50;
                case "Greater": return 100;
                case "Supreme": return 250;
                default: return 10;
            }
        }

        int CalculateItemValue(GoonzuItem item)
        {
            int baseValue = 10;

            // Rarity multiplier
            float rarityMultiplier = 1f;
            switch (item.rarity)
            {
                case GoonzuItem.ItemRarity.Common: rarityMultiplier = 1f; break;
                case GoonzuItem.ItemRarity.Uncommon: rarityMultiplier = 2f; break;
                case GoonzuItem.ItemRarity.Rare: rarityMultiplier = 5f; break;
                case GoonzuItem.ItemRarity.Epic: rarityMultiplier = 10f; break;
                case GoonzuItem.ItemRarity.Legendary: rarityMultiplier = 25f; break;
            }

            // Stat bonuses
            int statBonus = item.strengthBonus + item.dexterityBonus + item.constitutionBonus +
                           item.intelligenceBonus + item.wisdomBonus + item.charismaBonus;

            return Mathf.RoundToInt((baseValue + item.damage + item.armor + statBonus) * rarityMultiplier);
        }

        public GoonzuItem GenerateRandomItem(int level = 1, GoonzuItem.ItemType? forcedType = null)
        {
            GoonzuItem.ItemType itemType = forcedType ?? (GoonzuItem.ItemType)Random.Range(0, 5);

            switch (itemType)
            {
                case GoonzuItem.ItemType.Weapon:
                    string[] weaponTypes = { "Sword", "Axe", "Mace", "Dagger", "Bow", "Staff" };
                    string[] weaponMaterials = { "Iron", "Steel", "Silver", "Gold", "Mithril" };
                    string weaponType = weaponTypes[Random.Range(0, weaponTypes.Length)];
                    string weaponMaterial = weaponMaterials[Mathf.Min(level / 5, weaponMaterials.Length - 1)];
                    return CreateWeapon(weaponType, weaponMaterial);

                case GoonzuItem.ItemType.Armor:
                    string[] armorTypes = { "Helmet", "Chestplate", "Leggings", "Boots", "Shield" };
                    string[] armorMaterials = { "Leather", "Chain", "Plate", "Scale", "Mithril" };
                    string armorType = armorTypes[Random.Range(0, armorTypes.Length)];
                    string armorMaterial = armorMaterials[Mathf.Min(level / 5, armorMaterials.Length - 1)];
                    return CreateArmor(armorType, armorMaterial);

                case GoonzuItem.ItemType.Accessory:
                    string[] accessoryTypes = { "Ring", "Amulet", "Necklace", "Bracelet" };
                    string[] accessoryMaterials = { "Silver", "Gold", "Crystal", "Gem" };
                    string accessoryType = accessoryTypes[Random.Range(0, accessoryTypes.Length)];
                    string accessoryMaterial = accessoryMaterials[Mathf.Min(level / 5, accessoryMaterials.Length - 1)];
                    return CreateAccessory(accessoryType, accessoryMaterial);

                case GoonzuItem.ItemType.Consumable:
                    string[] potionTypes = { "Health", "Mana", "Strength", "Dexterity" };
                    string[] potionSizes = { "Small", "Medium", "Large" };
                    string potionType = potionTypes[Random.Range(0, potionTypes.Length)];
                    string potionSize = potionSizes[Mathf.Min(level / 5, potionSizes.Length - 1)];
                    return CreatePotion(potionType, potionSize);

                case GoonzuItem.ItemType.Material:
                    string[] materials = { "Iron Ore", "Copper Ore", "Leather", "Herb", "Crystal" };
                    return CreateMaterial(materials[Random.Range(0, materials.Length)]);

                default:
                    return CreateMaterial("Iron Ore");
            }
        }

        public GoonzuItem GenerateMagicItem(GoonzuItem baseItem)
        {
            GoonzuItem magicItem = new GoonzuItem(baseItem); // Copy constructor

            // Add prefix
            List<string> prefixes = new List<string>();
            switch (baseItem.itemType)
            {
                case GoonzuItem.ItemType.Weapon: prefixes = weaponPrefixes; break;
                case GoonzuItem.ItemType.Armor: prefixes = armorPrefixes; break;
                case GoonzuItem.ItemType.Accessory: prefixes = accessoryPrefixes; break;
            }

            if (prefixes.Count > 0)
            {
                string prefix = prefixes[Random.Range(0, prefixes.Count)];
                magicItem.itemName = $"{prefix} {magicItem.itemName}";
                magicItem.rarity = GoonzuItem.ItemRarity.Rare;
                magicItem.value *= 3;

                // Add random stat bonuses
                magicItem.strengthBonus += Random.Range(1, 3);
                magicItem.dexterityBonus += Random.Range(1, 3);
                magicItem.constitutionBonus += Random.Range(1, 3);
                magicItem.intelligenceBonus += Random.Range(1, 3);
                magicItem.wisdomBonus += Random.Range(1, 3);
                magicItem.charismaBonus += Random.Range(1, 3);
            }

            return magicItem;
        }

        public GoonzuItem GetItemByID(string itemID)
        {
            if (itemDatabase.ContainsKey(itemID))
            {
                return new GoonzuItem(itemDatabase[itemID]); // Return copy
            }
            return null;
        }

        public List<GoonzuItem> GetItemsByType(GoonzuItem.ItemType type)
        {
            List<GoonzuItem> items = new List<GoonzuItem>();
            foreach (var item in itemDatabase.Values)
            {
                if (item.itemType == type)
                {
                    items.Add(new GoonzuItem(item)); // Return copy
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
                    items.Add(new GoonzuItem(item)); // Return copy
                }
            }
            return items;
        }

        public GoonzuItem CreateLootDrop(int playerLevel, float dropChance = 1f)
        {
            if (Random.value > dropChance) return null;

            // Higher level players get better drops
            GoonzuItem.ItemRarity rarity = GoonzuItem.ItemRarity.Common;
            float rarityRoll = Random.value;

            if (rarityRoll < 0.6f) rarity = GoonzuItem.ItemRarity.Common;
            else if (rarityRoll < 0.85f) rarity = GoonzuItem.ItemRarity.Uncommon;
            else if (rarityRoll < 0.95f) rarity = GoonzuItem.ItemRarity.Rare;
            else if (rarityRoll < 0.99f) rarity = GoonzuItem.ItemRarity.Epic;
            else rarity = GoonzuItem.ItemRarity.Legendary;

            // Adjust for player level
            if (playerLevel < 5) rarity = GoonzuItem.ItemRarity.Common;
            else if (playerLevel < 10 && rarity > GoonzuItem.ItemRarity.Uncommon) rarity = GoonzuItem.ItemRarity.Uncommon;

            List<GoonzuItem> suitableItems = GetItemsByRarity(rarity);
            if (suitableItems.Count > 0)
            {
                return suitableItems[Random.Range(0, suitableItems.Count)];
            }

            // Fallback to random generation
            return GenerateRandomItem(playerLevel);
        }

        public void AddItemToDatabase(GoonzuItem item)
        {
            if (!itemDatabase.ContainsKey(item.itemID))
            {
                itemDatabase[item.itemID] = new GoonzuItem(item);
            }
        }

        public void RemoveItemFromDatabase(string itemID)
        {
            if (itemDatabase.ContainsKey(itemID))
            {
                itemDatabase.Remove(itemID);
            }
        }

        public int GetDatabaseSize()
        {
            return itemDatabase.Count;
        }

        public void ClearDatabase()
        {
            itemDatabase.Clear();
        }

        public void RegenerateDatabase()
        {
            ClearDatabase();
            GenerateItemDatabase();
        }
    }
}