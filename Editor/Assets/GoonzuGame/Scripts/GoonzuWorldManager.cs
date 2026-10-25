using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame
{
    public enum TerrainType
    {
        Grass,
        Dirt,
        Stone,
        Sand,
        Water,
        Snow,
        Forest,
        Mountain,
        Swamp,
        Desert,
        Plains,
        Hills,
        Cave,
        Road,
        Bridge
    }

    public enum BiomeType
    {
        Forest,
        Desert,
        Mountain,
        Plains,
        Swamp,
        Tundra,
        Coastal,
        Volcanic,
        Magical,
        Ruins,
        Dungeon
    }

    [System.Serializable]
    public class TerrainTile
    {
        public TerrainType terrainType;
        public Sprite terrainSprite;
        public bool isWalkable = true;
        public bool isSwimmable = false;
        public float movementCost = 1f;
        public string assetName;

        public TerrainTile(TerrainType type)
        {
            terrainType = type;
            assetName = type.ToString().ToLower();
            ConfigureTile();
        }

        void ConfigureTile()
        {
            switch (terrainType)
            {
                case TerrainType.Grass:
                    isWalkable = true;
                    movementCost = 1f;
                    break;
                case TerrainType.Dirt:
                    isWalkable = true;
                    movementCost = 1.1f;
                    break;
                case TerrainType.Stone:
                    isWalkable = true;
                    movementCost = 1.2f;
                    break;
                case TerrainType.Sand:
                    isWalkable = true;
                    movementCost = 1.5f;
                    break;
                case TerrainType.Water:
                    isWalkable = false;
                    isSwimmable = true;
                    movementCost = 3f;
                    break;
                case TerrainType.Snow:
                    isWalkable = true;
                    movementCost = 1.8f;
                    break;
                case TerrainType.Forest:
                    isWalkable = true;
                    movementCost = 2f;
                    break;
                case TerrainType.Mountain:
                    isWalkable = false;
                    movementCost = 10f;
                    break;
                case TerrainType.Swamp:
                    isWalkable = true;
                    movementCost = 2.5f;
                    break;
                case TerrainType.Road:
                    isWalkable = true;
                    movementCost = 0.8f;
                    break;
                case TerrainType.Bridge:
                    isWalkable = true;
                    movementCost = 1f;
                    break;
            }
        }
    }

    [System.Serializable]
    public class WorldZone
    {
        public string zoneName;
        public BiomeType biomeType;
        public Vector2Int zonePosition;
        public int width = 50;
        public int height = 50;
        public TerrainTile[,] terrainGrid;
        public List<GoonzuBuilding> buildings = new List<GoonzuBuilding>();
        public List<GoonzuCreature> creatures = new List<GoonzuCreature>();
        public List<GoonzuNPC> npcs = new List<GoonzuNPC>();

        public WorldZone(string name, BiomeType biome, Vector2Int position)
        {
            zoneName = name;
            biomeType = biome;
            zonePosition = position;
            terrainGrid = new TerrainTile[width, height];
            GenerateTerrain();
        }

        void GenerateTerrain()
        {
            // Simple terrain generation based on biome
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    terrainGrid[x, y] = GenerateTerrainTile(x, y);
                }
            }
        }

        TerrainTile GenerateTerrainTile(int x, int y)
        {
            TerrainType type = TerrainType.Grass; // Default

            switch (biomeType)
            {
                case BiomeType.Forest:
                    float forestNoise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                    if (forestNoise > 0.7f) type = TerrainType.Forest;
                    else if (forestNoise > 0.5f) type = TerrainType.Grass;
                    else type = TerrainType.Dirt;
                    break;

                case BiomeType.Desert:
                    type = Random.value > 0.1f ? TerrainType.Sand : TerrainType.Dirt;
                    break;

                case BiomeType.Mountain:
                    float mountainNoise = Mathf.PerlinNoise(x * 0.05f, y * 0.05f);
                    if (mountainNoise > 0.8f) type = TerrainType.Mountain;
                    else if (mountainNoise > 0.6f) type = TerrainType.Stone;
                    else type = TerrainType.Grass;
                    break;

                case BiomeType.Swamp:
                    type = Random.value > 0.3f ? TerrainType.Swamp : TerrainType.Water;
                    break;

                case BiomeType.Tundra:
                    type = Random.value > 0.2f ? TerrainType.Snow : TerrainType.Stone;
                    break;

                case BiomeType.Coastal:
                    if (y < 10) type = TerrainType.Water;
                    else if (y < 15) type = TerrainType.Sand;
                    else type = TerrainType.Grass;
                    break;

                default:
                    type = TerrainType.Grass;
                    break;
            }

            return new TerrainTile(type);
        }

        public TerrainTile GetTerrainAt(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                return terrainGrid[x, y];
            }
            return null;
        }

        public bool IsPositionWalkable(int x, int y)
        {
            TerrainTile tile = GetTerrainAt(x, y);
            return tile != null && tile.isWalkable;
        }

        public void AddBuilding(GoonzuBuilding building, Vector2Int position)
        {
            building.transform.position = new Vector3(position.x, position.y, 0);
            buildings.Add(building);
        }

        public void AddCreature(GoonzuCreature creature, Vector2Int position)
        {
            creature.transform.position = new Vector3(position.x, position.y, 0);
            creatures.Add(creature);
        }

        public void AddNPC(GoonzuNPC npc, Vector2Int position)
        {
            npc.transform.position = new Vector3(position.x, position.y, 0);
            npcs.Add(npc);
        }
    }

    public class GoonzuWorldManager : MonoBehaviour
    {
        public static GoonzuWorldManager Instance { get; private set; }

        [Header("World Settings")]
        public int worldWidth = 10; // Number of zones wide
        public int worldHeight = 10; // Number of zones high
        public int zoneSize = 50; // Size of each zone in tiles

        [Header("Zone Generation")]
        public List<BiomeType> availableBiomes = new List<BiomeType>();
        public Dictionary<Vector2Int, WorldZone> worldZones = new Dictionary<Vector2Int, WorldZone>();

        [Header("Current Zone")]
        public WorldZone currentZone;
        public Vector2Int currentZonePosition = Vector2Int.zero;

        [Header("Prefabs")]
        public GameObject terrainTilePrefab;
        public GameObject buildingPrefab;
        public GameObject creaturePrefab;
        public GameObject npcPrefab;

        private Transform worldContainer;
        private Dictionary<Vector2Int, GameObject> activeZoneObjects = new Dictionary<Vector2Int, GameObject>();

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

            worldContainer = new GameObject("WorldContainer").transform;
            worldContainer.parent = transform;

            InitializeBiomes();
        }

        void Start()
        {
            GenerateWorld();
            LoadZone(currentZonePosition);
        }

        void InitializeBiomes()
        {
            availableBiomes.AddRange(new BiomeType[] {
                BiomeType.Forest, BiomeType.Plains, BiomeType.Mountain,
                BiomeType.Desert, BiomeType.Swamp, BiomeType.Coastal
            });
        }

        void GenerateWorld()
        {
            for (int x = 0; x < worldWidth; x++)
            {
                for (int y = 0; y < worldHeight; y++)
                {
                    Vector2Int zonePos = new Vector2Int(x, y);
                    BiomeType biome = GetBiomeForPosition(zonePos);
                    string zoneName = GenerateZoneName(biome, zonePos);

                    WorldZone zone = new WorldZone(zoneName, biome, zonePos);
                    worldZones[zonePos] = zone;

                    // Generate some random buildings, creatures, and NPCs
                    GenerateZoneContent(zone);
                }
            }
        }

        BiomeType GetBiomeForPosition(Vector2Int position)
        {
            // Simple biome distribution based on position
            float noise = Mathf.PerlinNoise(position.x * 0.1f, position.y * 0.1f);

            if (noise < 0.2f) return BiomeType.Desert;
            else if (noise < 0.4f) return BiomeType.Mountain;
            else if (noise < 0.6f) return BiomeType.Swamp;
            else if (noise < 0.8f) return BiomeType.Coastal;
            else return BiomeType.Forest;
        }

        string GenerateZoneName(BiomeType biome, Vector2Int position)
        {
            string[] prefixes = { "North", "South", "East", "West", "Upper", "Lower", "Great", "Dark" };
            string[] suffixes = { "Lands", "Valley", "Hills", "Plains", "Forest", "Mountains" };

            string prefix = prefixes[Random.Range(0, prefixes.Length)];
            string suffix = suffixes[Random.Range(0, suffixes.Length)];

            return $"{prefix} {biome} {suffix}";
        }

        void GenerateZoneContent(WorldZone zone)
        {
            // Generate buildings
            int buildingCount = Random.Range(3, 8);
            for (int i = 0; i < buildingCount; i++)
            {
                Vector2Int pos = new Vector2Int(
                    Random.Range(5, zone.width - 5),
                    Random.Range(5, zone.height - 5)
                );

                if (zone.IsPositionWalkable(pos.x, pos.y))
                {
                    BuildingType buildingType = GetRandomBuildingType(zone.biomeType);
                    GameObject buildingObj = Instantiate(buildingPrefab, worldContainer);
                    GoonzuBuilding building = buildingObj.GetComponent<GoonzuBuilding>();

                    if (building != null)
                    {
                        building.buildingType = buildingType;
                        building.buildingName = GenerateBuildingName(buildingType);
                        zone.AddBuilding(building, pos);
                    }
                }
            }

            // Generate creatures
            int creatureCount = Random.Range(5, 15);
            for (int i = 0; i < creatureCount; i++)
            {
                Vector2Int pos = new Vector2Int(
                    Random.Range(0, zone.width),
                    Random.Range(0, zone.height)
                );

                if (zone.IsPositionWalkable(pos.x, pos.y))
                {
                    GameObject creatureObj = Instantiate(creaturePrefab, worldContainer);
                    GoonzuCreature creature = creatureObj.GetComponent<GoonzuCreature>();

                    if (creature != null)
                    {
                        creature.creatureType = GetRandomCreatureType(zone.biomeType);
                        zone.AddCreature(creature, pos);
                    }
                }
            }

            // Generate NPCs
            int npcCount = Random.Range(2, 6);
            for (int i = 0; i < npcCount; i++)
            {
                Vector2Int pos = new Vector2Int(
                    Random.Range(5, zone.width - 5),
                    Random.Range(5, zone.height - 5)
                );

                if (zone.IsPositionWalkable(pos.x, pos.y))
                {
                    GameObject npcObj = Instantiate(npcPrefab, worldContainer);
                    GoonzuNPC npc = npcObj.GetComponent<GoonzuNPC>();

                    if (npc != null)
                    {
                        npc.npcType = GetRandomNPCType();
                        zone.AddNPC(npc, pos);
                    }
                }
            }
        }

        BuildingType GetRandomBuildingType(BiomeType biome)
        {
            BuildingType[] commonBuildings = {
                BuildingType.TownHouse, BuildingType.BlacksmithShop, BuildingType.Tavern,
                BuildingType.Temple, BuildingType.Inn, BuildingType.MarketStall
            };

            BuildingType[] biomeBuildings = GetBiomeSpecificBuildings(biome);

            BuildingType[] allBuildings = new BuildingType[commonBuildings.Length + biomeBuildings.Length];
            commonBuildings.CopyTo(allBuildings, 0);
            biomeBuildings.CopyTo(allBuildings, commonBuildings.Length);

            return allBuildings[Random.Range(0, allBuildings.Length)];
        }

        BuildingType[] GetBiomeSpecificBuildings(BiomeType biome)
        {
            switch (biome)
            {
                case BiomeType.Mountain:
                    return new BuildingType[] { BuildingType.MineEntrance, BuildingType.Watchtower };
                case BiomeType.Coastal:
                    return new BuildingType[] { BuildingType.Stable, BuildingType.Lumberyard };
                case BiomeType.Swamp:
                    return new BuildingType[] { BuildingType.Apothecary, BuildingType.Mill };
                default:
                    return new BuildingType[] { BuildingType.Library, BuildingType.GuildHall };
            }
        }

        string GenerateBuildingName(BuildingType type)
        {
            string[] adjectives = { "Old", "Grand", "Royal", "Ancient", "Mystical", "Rustic" };
            string adjective = adjectives[Random.Range(0, adjectives.Length)];
            return $"{adjective} {type}";
        }

        GoonzuCreature.CreatureType GetRandomCreatureType(BiomeType biome)
        {
            switch (biome)
            {
                case BiomeType.Forest:
                    return Random.value > 0.5f ? GoonzuCreature.CreatureType.Wolf : GoonzuCreature.CreatureType.Spider;
                case BiomeType.Mountain:
                    return Random.value > 0.5f ? GoonzuCreature.CreatureType.Golem : GoonzuCreature.CreatureType.Yeti;
                case BiomeType.Swamp:
                    return Random.value > 0.5f ? GoonzuCreature.CreatureType.Troll : GoonzuCreature.CreatureType.Slime;
                case BiomeType.Desert:
                    return Random.value > 0.5f ? GoonzuCreature.CreatureType.Scorpion : GoonzuCreature.CreatureType.Sandworm;
                default:
                    return GoonzuCreature.CreatureType.Goblin;
            }
        }

        GoonzuNPC.NPCType GetRandomNPCType()
        {
            GoonzuNPC.NPCType[] types = {
                GoonzuNPC.NPCType.Villager, GoonzuNPC.NPCType.Merchant,
                GoonzuNPC.NPCType.Guard, GoonzuNPC.NPCType.Priest
            };
            return types[Random.Range(0, types.Length)];
        }

        public void LoadZone(Vector2Int zonePosition)
        {
            // Unload current zone
            if (currentZone != null)
            {
                UnloadZone(currentZonePosition);
            }

            // Load new zone
            if (worldZones.ContainsKey(zonePosition))
            {
                currentZone = worldZones[zonePosition];
                currentZonePosition = zonePosition;

                GameObject zoneContainer = new GameObject($"Zone_{zonePosition.x}_{zonePosition.y}");
                zoneContainer.transform.parent = worldContainer;
                activeZoneObjects[zonePosition] = zoneContainer;

                // Create terrain tiles
                CreateTerrainTiles(zoneContainer.transform);

                // Position camera to zone center
                Camera.main.transform.position = new Vector3(
                    zonePosition.x * zoneSize + zoneSize / 2f,
                    zonePosition.y * zoneSize + zoneSize / 2f,
                    -10f
                );

                Debug.Log($"Loaded zone: {currentZone.zoneName} at {zonePosition}");
            }
        }

        void CreateTerrainTiles(Transform parent)
        {
            for (int x = 0; x < currentZone.width; x++)
            {
                for (int y = 0; y < currentZone.height; y++)
                {
                    TerrainTile tile = currentZone.GetTerrainAt(x, y);
                    if (tile != null)
                    {
                        GameObject tileObj = Instantiate(terrainTilePrefab, parent);
                        tileObj.transform.position = new Vector3(
                            currentZonePosition.x * zoneSize + x,
                            currentZonePosition.y * zoneSize + y,
                            0
                        );

                        SpriteRenderer renderer = tileObj.GetComponent<SpriteRenderer>();
                        if (renderer != null)
                        {
                            tile.terrainSprite = GoonzuAssetManager.Instance.GetSprite($"Terrain/{tile.assetName}");
                            if (tile.terrainSprite != null)
                            {
                                renderer.sprite = tile.terrainSprite;
                            }
                        }
                    }
                }
            }
        }

        void UnloadZone(Vector2Int zonePosition)
        {
            if (activeZoneObjects.ContainsKey(zonePosition))
            {
                Destroy(activeZoneObjects[zonePosition]);
                activeZoneObjects.Remove(zonePosition);
            }
        }

        public WorldZone GetCurrentZone()
        {
            return currentZone;
        }

        public TerrainTile GetTerrainAtWorldPosition(Vector3 worldPosition)
        {
            Vector2Int zonePos = new Vector2Int(
                Mathf.FloorToInt(worldPosition.x / zoneSize),
                Mathf.FloorToInt(worldPosition.y / zoneSize)
            );

            if (worldZones.ContainsKey(zonePos))
            {
                WorldZone zone = worldZones[zonePos];
                int localX = Mathf.FloorToInt(worldPosition.x) % zoneSize;
                int localY = Mathf.FloorToInt(worldPosition.y) % zoneSize;

                return zone.GetTerrainAt(localX, localY);
            }

            return null;
        }

        public bool IsPositionWalkable(Vector3 worldPosition)
        {
            TerrainTile tile = GetTerrainAtWorldPosition(worldPosition);
            return tile != null && tile.isWalkable;
        }

        public void TravelToZone(Vector2Int zonePosition)
        {
            if (worldZones.ContainsKey(zonePosition))
            {
                LoadZone(zonePosition);
            }
        }

        public Vector2Int GetZonePosition(Vector3 worldPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt(worldPosition.x / zoneSize),
                Mathf.FloorToInt(worldPosition.y / zoneSize)
            );
        }

        public List<Vector2Int> GetAdjacentZones(Vector2Int zonePosition)
        {
            List<Vector2Int> adjacent = new List<Vector2Int>();

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;

                    Vector2Int adjacentPos = zonePosition + new Vector2Int(dx, dy);
                    if (worldZones.ContainsKey(adjacentPos))
                    {
                        adjacent.Add(adjacentPos);
                    }
                }
            }

            return adjacent;
        }
    }
}