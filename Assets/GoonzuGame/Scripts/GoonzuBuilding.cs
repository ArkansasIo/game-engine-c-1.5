using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame
{
    public enum BuildingType
    {
        TownHouse,
        Castle,
        Tower,
        Keep,
        BlacksmithShop,
        Tavern,
        Temple,
        Library,
        MarketStall,
        Stable,
        Barracks,
        GuildHall,
        Apothecary,
        Bakery,
        TailorShop,
        Jeweler,
        Armory,
        Inn,
        Chapel,
        Watchtower,
        Gatehouse,
        Bridge,
        Mill,
        Farmhouse,
        Lumberyard,
        MineEntrance,
        DungeonEntrance,
        Portal,
        Obelisk,
        Fountain
    }

    public enum BuildingState
    {
        Normal,
        Damaged,
        Destroyed,
        UnderConstruction,
        Abandoned
    }

    [System.Serializable]
    public class BuildingFunction
    {
        public string functionName;
        public System.Action<GoonzuCharacter> interactAction;
        public bool requiresQuest = false;
        public int requiredLevel = 1;
        public string description;
    }

    public class GoonzuBuilding : MonoBehaviour
    {
        [Header("Building Identity")]
        public string buildingName;
        public BuildingType buildingType;
        public BuildingState buildingState = BuildingState.Normal;

        [Header("Building Properties")]
        public int maxHealth = 100;
        public int currentHealth = 100;
        public bool isInteractive = true;
        public float interactionRange = 3f;
        public List<BuildingFunction> functions = new List<BuildingFunction>();

        [Header("Visual Settings")]
        public Sprite normalSprite;
        public Sprite damagedSprite;
        public Sprite destroyedSprite;
        public ParticleSystem constructionEffect;
        public ParticleSystem destructionEffect;

        private SpriteRenderer spriteRenderer;
        private string assetName;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        void Start()
        {
            InitializeBuilding();
        }

        void InitializeBuilding()
        {
            assetName = buildingType.ToString().ToLower();

            // Load building sprite
            if (normalSprite == null)
            {
                normalSprite = GoonzuAssetManager.Instance.GetSprite($"Buildings/{assetName}");
                if (normalSprite != null)
                {
                    spriteRenderer.sprite = normalSprite;
                }
            }

            // Initialize building functions based on type
            InitializeFunctions();

            // Set up collider for interaction
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = gameObject.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
            }
        }

        void InitializeFunctions()
        {
            switch (buildingType)
            {
                case BuildingType.BlacksmithShop:
                    AddFunction("Shop", (character) => OpenShop(character), "Buy weapons and armor");
                    AddFunction("Repair", (character) => RepairItems(character), "Repair damaged equipment", requiredLevel: 1);
                    break;

                case BuildingType.Tavern:
                    AddFunction("Rest", (character) => Rest(character), "Rest and recover health/mana");
                    AddFunction("Drink", (character) => BuyDrink(character), "Buy drinks and socialize");
                    AddFunction("Quest", (character) => GetTavernQuest(character), "Get quests from patrons", requiresQuest: true);
                    break;

                case BuildingType.Temple:
                    AddFunction("Pray", (character) => Pray(character), "Pray for blessings");
                    AddFunction("Heal", (character) => TempleHeal(character), "Receive healing", requiredLevel: 1);
                    AddFunction("Donate", (character) => Donate(character), "Make donations for blessings");
                    break;

                case BuildingType.Inn:
                    AddFunction("Sleep", (character) => Sleep(character), "Sleep overnight to recover");
                    AddFunction("Store Items", (character) => OpenBank(character), "Store items safely");
                    break;

                case BuildingType.Library:
                    AddFunction("Study", (character) => Study(character), "Learn new skills");
                    AddFunction("Research", (character) => Research(character), "Research lore and history");
                    break;

                case BuildingType.Stable:
                    AddFunction("Buy Mount", (character) => BuyMount(character), "Purchase a mount");
                    AddFunction("Stable Mount", (character) => StableMount(character), "Store your mount");
                    break;

                case BuildingType.MarketStall:
                    AddFunction("Shop", (character) => OpenMarketShop(character), "Buy general goods");
                    AddFunction("Sell", (character) => SellGoods(character), "Sell your items");
                    break;

                case BuildingType.GuildHall:
                    AddFunction("Join Guild", (character) => JoinGuild(character), "Join an adventurer's guild");
                    AddFunction("Guild Quests", (character) => GuildQuests(character), "Take guild quests");
                    AddFunction("Guild Bank", (character) => GuildBank(character), "Access guild storage");
                    break;

                case BuildingType.Apothecary:
                    AddFunction("Buy Potions", (character) => BuyPotions(character), "Purchase healing potions");
                    AddFunction("Mix Potion", (character) => MixPotion(character), "Create custom potions", requiredLevel: 5);
                    break;

                case BuildingType.Barracks:
                    AddFunction("Train", (character) => Train(character), "Train combat skills");
                    AddFunction("Recruit", (character) => Recruit(character), "Recruit NPC companions", requiredLevel: 10);
                    break;

                case BuildingType.Castle:
                    AddFunction("Audience", (character) => RoyalAudience(character), "Speak with the ruler", requiredLevel: 15);
                    AddFunction("Quest", (character) => RoyalQuest(character), "Receive royal quests", requiresQuest: true);
                    break;

                case BuildingType.DungeonEntrance:
                    AddFunction("Enter", (character) => EnterDungeon(character), "Enter the dungeon", requiredLevel: 5);
                    break;

                case BuildingType.Portal:
                    AddFunction("Teleport", (character) => UsePortal(character), "Travel to another location", requiredLevel: 10);
                    break;

                default:
                    // Generic buildings have basic interaction
                    AddFunction("Enter", (character) => EnterBuilding(character), "Enter the building");
                    break;
            }
        }

        void AddFunction(string name, System.Action<GoonzuCharacter> action, string description = "", bool requiresQuest = false, int requiredLevel = 1)
        {
            BuildingFunction function = new BuildingFunction
            {
                functionName = name,
                interactAction = action,
                requiresQuest = requiresQuest,
                requiredLevel = requiredLevel,
                description = description
            };
            functions.Add(function);
        }

        // Building function implementations
        void OpenShop(GoonzuCharacter character)
        {
            Debug.Log($"Opening {buildingType} shop for {character.characterName}");
            GoonzuUIManager.Instance.ShowPanel("trade");
        }

        void RepairItems(GoonzuCharacter character)
        {
            Debug.Log($"Repairing items for {character.characterName}");
            // Implement repair logic
        }

        void Rest(GoonzuCharacter character)
        {
            character.Heal(character.stats.maxHealth / 2);
            character.stats.RestoreMana(character.stats.maxMana / 2);
            Debug.Log($"{character.characterName} rests and recovers health and mana");
        }

        void BuyDrink(GoonzuCharacter character)
        {
            character.Heal(20);
            Debug.Log($"{character.characterName} buys a drink and feels refreshed");
        }

        void GetTavernQuest(GoonzuCharacter character)
        {
            Debug.Log($"Offering tavern quest to {character.characterName}");
        }

        void Pray(GoonzuCharacter character)
        {
            // Random blessing
            int blessing = Random.Range(0, 3);
            switch (blessing)
            {
                case 0:
                    character.stats.strength += 1;
                    Debug.Log($"{character.characterName} receives strength blessing");
                    break;
                case 1:
                    character.stats.wisdom += 1;
                    Debug.Log($"{character.characterName} receives wisdom blessing");
                    break;
                case 2:
                    character.stats.criticalChance += 0.05f;
                    Debug.Log($"{character.characterName} receives critical blessing");
                    break;
            }
        }

        void TempleHeal(GoonzuCharacter character)
        {
            character.Heal(character.stats.maxHealth);
            Debug.Log($"{character.characterName} is fully healed by divine power");
        }

        void Donate(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} makes a donation");
        }

        void Sleep(GoonzuCharacter character)
        {
            character.Heal(character.stats.maxHealth);
            character.stats.RestoreMana(character.stats.maxMana);
            Debug.Log($"{character.characterName} sleeps and fully recovers");
        }

        void OpenBank(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} accesses item storage");
        }

        void Study(GoonzuCharacter character)
        {
            character.GainExperience(50);
            Debug.Log($"{character.characterName} studies and gains experience");
        }

        void Research(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} researches ancient lore");
        }

        void BuyMount(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} purchases a mount");
        }

        void StableMount(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} stables their mount");
        }

        void OpenMarketShop(GoonzuCharacter character)
        {
            GoonzuUIManager.Instance.ShowPanel("trade");
        }

        void SellGoods(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} sells goods at the market");
        }

        void JoinGuild(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} joins the adventurer's guild");
        }

        void GuildQuests(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} takes guild quests");
        }

        void GuildBank(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} accesses guild bank");
        }

        void BuyPotions(GoonzuCharacter character)
        {
            GoonzuUIManager.Instance.ShowPanel("trade");
        }

        void MixPotion(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} mixes a custom potion");
        }

        void Train(GoonzuCharacter character)
        {
            character.GainExperience(25);
            Debug.Log($"{character.characterName} trains and improves skills");
        }

        void Recruit(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} recruits an NPC companion");
        }

        void RoyalAudience(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} has an audience with the ruler");
        }

        void RoyalQuest(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} receives a royal quest");
        }

        void EnterDungeon(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} enters the dungeon");
            // Load dungeon scene
        }

        void UsePortal(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} uses the portal");
            // Teleport to new location
        }

        void EnterBuilding(GoonzuCharacter character)
        {
            Debug.Log($"{character.characterName} enters {buildingName}");
        }

        // Public API methods
        public void Interact(GoonzuCharacter character)
        {
            if (!isInteractive || functions.Count == 0) return;

            if (functions.Count == 1)
            {
                // Single function - execute directly
                functions[0].interactAction(character);
            }
            else
            {
                // Multiple functions - show selection menu
                ShowFunctionMenu(character);
            }
        }

        void ShowFunctionMenu(GoonzuCharacter character)
        {
            Debug.Log($"Available functions at {buildingName}:");
            for (int i = 0; i < functions.Count; i++)
            {
                BuildingFunction func = functions[i];
                string levelReq = func.requiredLevel > 1 ? $" (Level {func.requiredLevel}+)" : "";
                string questReq = func.requiresQuest ? " (Quest Required)" : "";
                Debug.Log($"{i + 1}. {func.functionName}{levelReq}{questReq} - {func.description}");
            }
        }

        public void TakeDamage(int damage)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);

            if (currentHealth <= 0)
            {
                buildingState = BuildingState.Destroyed;
                UpdateVisuals();
                if (destructionEffect) destructionEffect.Play();
            }
            else if (currentHealth <= maxHealth * 0.3f)
            {
                buildingState = BuildingState.Damaged;
                UpdateVisuals();
            }
        }

        void UpdateVisuals()
        {
            switch (buildingState)
            {
                case BuildingState.Normal:
                    if (normalSprite) spriteRenderer.sprite = normalSprite;
                    break;
                case BuildingState.Damaged:
                    if (damagedSprite) spriteRenderer.sprite = damagedSprite;
                    else if (normalSprite) spriteRenderer.sprite = normalSprite;
                    break;
                case BuildingState.Destroyed:
                    if (destroyedSprite) spriteRenderer.sprite = destroyedSprite;
                    else if (normalSprite) spriteRenderer.sprite = normalSprite;
                    break;
            }
        }

        public void Repair(int repairAmount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + repairAmount);
            if (currentHealth > maxHealth * 0.3f)
            {
                buildingState = BuildingState.Normal;
                UpdateVisuals();
            }
        }

        public bool CanInteract(GoonzuCharacter character)
        {
            if (!isInteractive) return false;
            if (buildingState == BuildingState.Destroyed) return false;

            return Vector3.Distance(transform.position, character.transform.position) <= interactionRange;
        }

        public BuildingType GetBuildingType()
        {
            return buildingType;
        }

        public string GetAssetName()
        {
            return assetName;
        }

        public List<BuildingFunction> GetAvailableFunctions(GoonzuCharacter character)
        {
            List<BuildingFunction> available = new List<BuildingFunction>();

            foreach (var function in functions)
            {
                if (character.stats.level >= function.requiredLevel)
                {
                    // Check quest requirements (simplified)
                    if (!function.requiresQuest)
                    {
                        available.Add(function);
                    }
                }
            }

            return available;
        }
    }
}