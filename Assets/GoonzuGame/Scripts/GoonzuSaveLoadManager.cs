using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace GoonzuGame
{
    [System.Serializable]
    public class GameSaveData
    {
        public string saveName;
        public string saveDate;
        public int gameVersion;

        // Player data
        public CharacterSaveData playerData;

        // World data
        public WorldSaveData worldData;

        // Game state
        public GameStateSaveData gameStateData;

        public GameSaveData(string name)
        {
            saveName = name;
            saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            gameVersion = 1;

            playerData = new CharacterSaveData();
            worldData = new WorldSaveData();
            gameStateData = new GameStateSaveData();
        }
    }

    [System.Serializable]
    public class CharacterSaveData
    {
        public string characterName;
        public GoonzuCharacter.CharacterRace race;
        public GoonzuCharacter.CharacterClass characterClass;
        public int level;
        public int experience;
        public int gold;

        // Stats
        public int strength;
        public int dexterity;
        public int constitution;
        public int intelligence;
        public int wisdom;
        public int charisma;

        // Health and mana
        public int currentHealth;
        public int maxHealth;
        public int currentMana;
        public int maxMana;

        // Equipment
        public string weaponID;
        public string armorID;
        public string accessoryID;

        // Inventory
        public List<string> inventoryItemIDs = new List<string>();
        public List<int> inventoryItemCounts = new List<int>();

        // Position
        public float posX;
        public float posY;
        public float posZ;

        // Quests
        public List<string> activeQuestIDs = new List<string>();
        public List<string> completedQuestIDs = new List<string>();
    }

    [System.Serializable]
    public class WorldSaveData
    {
        public string currentZoneName;
        public int zonePosX;
        public int zonePosY;

        // World zones (simplified - could be expanded)
        public List<ZoneSaveData> zones = new List<ZoneSaveData>();
    }

    [System.Serializable]
    public class ZoneSaveData
    {
        public string zoneName;
        public int posX;
        public int posY;
        public GoonzuWorldManager.BiomeType biomeType;

        // Buildings
        public List<BuildingSaveData> buildings = new List<BuildingSaveData>();

        // Creatures
        public List<CreatureSaveData> creatures = new List<CreatureSaveData>();

        // NPCs
        public List<NPCSaveData> npcs = new List<NPCSaveData>();
    }

    [System.Serializable]
    public class BuildingSaveData
    {
        public string buildingName;
        public GoonzuBuilding.BuildingType buildingType;
        public GoonzuBuilding.BuildingState buildingState;
        public int currentHealth;
        public float posX;
        public float posY;
        public float posZ;
    }

    [System.Serializable]
    public class CreatureSaveData
    {
        public GoonzuCreature.CreatureType creatureType;
        public int currentHealth;
        public float posX;
        public float posY;
        public float posZ;
        public GoonzuCreature.AIBehavior currentBehavior;
    }

    [System.Serializable]
    public class NPCSaveData
    {
        public GoonzuNPC.NPCType npcType;
        public string npcName;
        public float posX;
        public float posY;
        public float posZ;
        public List<string> shopItems = new List<string>();
    }

    [System.Serializable]
    public class GameStateSaveData
    {
        public float gameTime;
        public int dayCount;
        public string weatherType;
        public float timeOfDay; // 0-24 hours

        // Global flags
        public Dictionary<string, bool> globalFlags = new Dictionary<string, bool>();

        // Completed achievements
        public List<string> completedAchievements = new List<string>();
    }

    public class GoonzuSaveLoadManager : MonoBehaviour
    {
        public static GoonzuSaveLoadManager Instance { get; private set; }

        [Header("Save Settings")]
        public string saveDirectory = "Saves";
        public string saveFileExtension = ".goonzu";
        public int maxSaveSlots = 10;

        [Header("Auto Save")]
        public bool enableAutoSave = true;
        public float autoSaveInterval = 300f; // 5 minutes
        private float lastAutoSaveTime = 0f;

        private string savePath;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            savePath = Path.Combine(Application.persistentDataPath, saveDirectory);
            Directory.CreateDirectory(savePath);
        }

        void Update()
        {
            if (enableAutoSave && Time.time - lastAutoSaveTime >= autoSaveInterval)
            {
                AutoSave();
                lastAutoSaveTime = Time.time;
            }
        }

        public void SaveGame(string saveName)
        {
            GameSaveData saveData = CreateSaveData(saveName);
            string filePath = GetSaveFilePath(saveName);

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    formatter.Serialize(stream, saveData);
                }

                Debug.Log($"Game saved successfully: {saveName}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }

        public bool LoadGame(string saveName)
        {
            string filePath = GetSaveFilePath(saveName);

            if (!File.Exists(filePath))
            {
                Debug.LogError($"Save file not found: {saveName}");
                return false;
            }

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                {
                    GameSaveData saveData = formatter.Deserialize(stream) as GameSaveData;
                    if (saveData != null)
                    {
                        LoadSaveData(saveData);
                        Debug.Log($"Game loaded successfully: {saveName}");
                        return true;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}");
            }

            return false;
        }

        public void AutoSave()
        {
            SaveGame("AutoSave");
        }

        public void QuickSave()
        {
            SaveGame("QuickSave");
        }

        public bool QuickLoad()
        {
            return LoadGame("QuickSave");
        }

        public List<string> GetAvailableSaves()
        {
            List<string> saves = new List<string>();

            if (Directory.Exists(savePath))
            {
                string[] files = Directory.GetFiles(savePath, $"*{saveFileExtension}");
                foreach (string file in files)
                {
                    string saveName = Path.GetFileNameWithoutExtension(file);
                    saves.Add(saveName);
                }
            }

            return saves;
        }

        public void DeleteSave(string saveName)
        {
            string filePath = GetSaveFilePath(saveName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"Save deleted: {saveName}");
            }
        }

        private GameSaveData CreateSaveData(string saveName)
        {
            GameSaveData saveData = new GameSaveData(saveName);

            // Save player data
            GoonzuCharacter player = GoonzuGameManager.Instance.GetPlayer();
            if (player != null)
            {
                saveData.playerData = CreateCharacterSaveData(player);
            }

            // Save world data
            saveData.worldData = CreateWorldSaveData();

            // Save game state
            saveData.gameStateData = CreateGameStateSaveData();

            return saveData;
        }

        private CharacterSaveData CreateCharacterSaveData(GoonzuCharacter character)
        {
            CharacterSaveData data = new CharacterSaveData();

            data.characterName = character.characterName;
            data.race = character.race;
            data.characterClass = character.characterClass;
            data.level = character.stats.level;
            data.experience = character.stats.experience;
            data.gold = character.stats.gold;

            data.strength = character.stats.strength;
            data.dexterity = character.stats.dexterity;
            data.constitution = character.stats.constitution;
            data.intelligence = character.stats.intelligence;
            data.wisdom = character.stats.wisdom;
            data.charisma = character.stats.charisma;

            data.currentHealth = character.stats.currentHealth;
            data.maxHealth = character.stats.maxHealth;
            data.currentMana = character.stats.currentMana;
            data.maxMana = character.stats.maxMana;

            // Equipment
            if (character.equippedWeapon != null) data.weaponID = character.equippedWeapon.itemID;
            if (character.equippedArmor != null) data.armorID = character.equippedArmor.itemID;
            if (character.equippedAccessory != null) data.accessoryID = character.equippedAccessory.itemID;

            // Inventory
            foreach (var item in character.inventory)
            {
                data.inventoryItemIDs.Add(item.itemID);
                data.inventoryItemCounts.Add(item.stackCount);
            }

            // Position
            data.posX = character.transform.position.x;
            data.posY = character.transform.position.y;
            data.posZ = character.transform.position.z;

            // Quests
            foreach (var quest in GoonzuQuestManager.Instance.GetActiveQuests())
            {
                data.activeQuestIDs.Add(quest.questID);
            }
            foreach (var quest in GoonzuQuestManager.Instance.GetCompletedQuests())
            {
                data.completedQuestIDs.Add(quest.questID);
            }

            return data;
        }

        private WorldSaveData CreateWorldSaveData()
        {
            WorldSaveData data = new WorldSaveData();

            // Current zone
            WorldZone currentZone = GoonzuWorldManager.Instance.GetCurrentZone();
            if (currentZone != null)
            {
                data.currentZoneName = currentZone.zoneName;
                data.zonePosX = currentZone.zonePosition.x;
                data.zonePosY = currentZone.zonePosition.y;
            }

            // Save all zones (simplified - in a full implementation, you'd save all zones)
            foreach (var zonePair in GoonzuWorldManager.Instance.worldZones)
            {
                WorldZone zone = zonePair.Value;
                ZoneSaveData zoneData = new ZoneSaveData();
                zoneData.zoneName = zone.zoneName;
                zoneData.posX = zone.zonePosition.x;
                zoneData.posY = zone.zonePosition.y;
                zoneData.biomeType = zone.biomeType;

                // Save buildings
                foreach (var building in zone.buildings)
                {
                    BuildingSaveData buildingData = new BuildingSaveData();
                    buildingData.buildingName = building.buildingName;
                    buildingData.buildingType = building.buildingType;
                    buildingData.buildingState = building.buildingState;
                    buildingData.currentHealth = building.currentHealth;
                    buildingData.posX = building.transform.position.x;
                    buildingData.posY = building.transform.position.y;
                    buildingData.posZ = building.transform.position.z;
                    zoneData.buildings.Add(buildingData);
                }

                // Save creatures
                foreach (var creature in zone.creatures)
                {
                    CreatureSaveData creatureData = new CreatureSaveData();
                    creatureData.creatureType = creature.creatureType;
                    creatureData.currentHealth = creature.currentHealth;
                    creatureData.posX = creature.transform.position.x;
                    creatureData.posY = creature.transform.position.y;
                    creatureData.posZ = creature.transform.position.z;
                    creatureData.currentBehavior = creature.currentBehavior;
                    zoneData.creatures.Add(creatureData);
                }

                // Save NPCs
                foreach (var npc in zone.npcs)
                {
                    NPCSaveData npcData = new NPCSaveData();
                    npcData.npcType = npc.npcType;
                    npcData.npcName = npc.npcName;
                    npcData.posX = npc.transform.position.x;
                    npcData.posY = npc.transform.position.y;
                    npcData.posZ = npc.transform.position.z;

                    // Save shop items if applicable
                    if (npc.shopInventory != null)
                    {
                        foreach (var item in npc.shopInventory)
                        {
                            npcData.shopItems.Add(item.itemID);
                        }
                    }

                    zoneData.npcs.Add(npcData);
                }

                data.zones.Add(zoneData);
            }

            return data;
        }

        private GameStateSaveData CreateGameStateSaveData()
        {
            GameStateSaveData data = new GameStateSaveData();

            // Game time (simplified)
            data.gameTime = Time.time;
            data.dayCount = 1; // Would track actual game days
            data.weatherType = "Clear"; // Would track actual weather
            data.timeOfDay = 12f; // Would track actual time of day

            // Global flags (example)
            data.globalFlags["tutorial_completed"] = true;
            data.globalFlags["first_quest_completed"] = false;

            // Completed achievements (example)
            data.completedAchievements.Add("first_steps");
            data.completedAchievements.Add("first_kill");

            return data;
        }

        private void LoadSaveData(GameSaveData saveData)
        {
            // Load player data
            LoadCharacterData(saveData.playerData);

            // Load world data
            LoadWorldData(saveData.worldData);

            // Load game state
            LoadGameStateData(saveData.gameStateData);
        }

        private void LoadCharacterData(CharacterSaveData data)
        {
            GoonzuCharacter player = GoonzuGameManager.Instance.GetPlayer();
            if (player == null) return;

            player.characterName = data.characterName;
            player.race = data.race;
            player.characterClass = data.characterClass;
            player.stats.level = data.level;
            player.stats.experience = data.experience;
            player.stats.gold = data.gold;

            player.stats.strength = data.strength;
            player.stats.dexterity = data.dexterity;
            player.stats.constitution = data.constitution;
            player.stats.intelligence = data.intelligence;
            player.stats.wisdom = data.wisdom;
            player.stats.charisma = data.charisma;

            player.stats.currentHealth = data.currentHealth;
            player.stats.maxHealth = data.maxHealth;
            player.stats.currentMana = data.currentMana;
            player.stats.maxMana = data.maxMana;

            // Position
            player.transform.position = new Vector3(data.posX, data.posY, data.posZ);

            // Equipment and inventory would need to be loaded from item database
            // Quests would need to be restored from quest manager

            Debug.Log($"Loaded character: {player.characterName}");
        }

        private void LoadWorldData(WorldSaveData data)
        {
            // Load current zone
            GoonzuWorldManager.Instance.TravelToZone(new Vector2Int(data.zonePosX, data.zonePosY));

            // Load zone data (simplified)
            foreach (var zoneData in data.zones)
            {
                // This would recreate buildings, creatures, and NPCs in the zone
                // Implementation would depend on how the world generation works
            }

            Debug.Log($"Loaded world data for zone: {data.currentZoneName}");
        }

        private void LoadGameStateData(GameStateSaveData data)
        {
            // Restore game time, weather, etc.
            Debug.Log($"Loaded game state: Day {data.dayCount}, Time {data.timeOfDay}");
        }

        private string GetSaveFilePath(string saveName)
        {
            return Path.Combine(savePath, saveName + saveFileExtension);
        }

        public string GetSaveInfo(string saveName)
        {
            string filePath = GetSaveFilePath(saveName);
            if (!File.Exists(filePath)) return "Save not found";

            FileInfo fileInfo = new FileInfo(filePath);
            return $"Size: {fileInfo.Length} bytes, Modified: {fileInfo.LastWriteTime}";
        }

        public void ExportSaveToJSON(string saveName)
        {
            GameSaveData saveData = CreateSaveData(saveName);
            string json = JsonUtility.ToJson(saveData, true);
            string jsonPath = Path.Combine(savePath, saveName + ".json");

            File.WriteAllText(jsonPath, json);
            Debug.Log($"Save exported to JSON: {jsonPath}");
        }

        public bool ImportSaveFromJSON(string jsonPath)
        {
            if (!File.Exists(jsonPath)) return false;

            try
            {
                string json = File.ReadAllText(jsonPath);
                GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

                if (saveData != null)
                {
                    LoadSaveData(saveData);
                    Debug.Log("Save imported from JSON successfully");
                    return true;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to import save from JSON: {e.Message}");
            }

            return false;
        }
    }
}