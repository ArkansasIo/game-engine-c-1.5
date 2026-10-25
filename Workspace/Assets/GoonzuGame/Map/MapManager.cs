using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame.Map
{
    [System.Serializable]
    public class MapLocation
    {
        public string Name;
        public Vector3 Position;
        public string Description;
        public bool IsExplored;
        public List<string> ConnectedLocations;

        public MapLocation(string name, Vector3 position, string description)
        {
            Name = name;
            Position = position;
            Description = description;
            IsExplored = false;
            ConnectedLocations = new List<string>();
        }
    }

    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }

        public Dictionary<string, MapLocation> Locations = new Dictionary<string, MapLocation>();
        public string CurrentLocation;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeMap();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeMap()
        {
            // Add sample locations
            Locations["Town"] = new MapLocation("Town", new Vector3(0, 0, 0), "A bustling town");
            Locations["Forest"] = new MapLocation("Forest", new Vector3(10, 0, 0), "A dense forest");
            Locations["Dungeon"] = new MapLocation("Dungeon", new Vector3(20, 0, 0), "A dark dungeon");

            // Connect locations
            Locations["Town"].ConnectedLocations.Add("Forest");
            Locations["Forest"].ConnectedLocations.Add("Town");
            Locations["Forest"].ConnectedLocations.Add("Dungeon");
            Locations["Dungeon"].ConnectedLocations.Add("Forest");

            CurrentLocation = "Town";
        }

        public void LoadMap(string mapName)
        {
            if (Locations.ContainsKey(mapName))
            {
                CurrentLocation = mapName;
                var location = Locations[mapName];
                location.IsExplored = true;
                // Load scene or move camera
                Debug.Log($"Loaded map: {mapName} at {location.Position}");
                // In Unity, you might load a scene or move the player
            }
            else
            {
                Debug.Log($"Map not found: {mapName}");
            }
        }

        public void TravelTo(string locationName)
        {
            if (Locations.ContainsKey(locationName) && Locations[CurrentLocation].ConnectedLocations.Contains(locationName))
            {
                LoadMap(locationName);
                // Update player position
                var player = FindObjectOfType<GoonzuGame.Characters.Character>();
                if (player != null)
                {
                    player.Position = Locations[locationName].Position;
                    player.transform.position = player.Position;
                }
            }
            else
            {
                Debug.Log($"Cannot travel to {locationName}");
            }
        }

        public List<string> GetAvailableLocations()
        {
            return Locations[CurrentLocation].ConnectedLocations;
        }

        public void DiscoverLocation(string locationName)
        {
            if (Locations.ContainsKey(locationName))
            {
                Locations[locationName].IsExplored = true;
                Debug.Log($"Discovered location: {locationName}");
            }
        }

        public void UpdateMapProgress()
        {
            // Update mini-map or world map UI
            GoonzuGame.Quests.QuestManager.Instance.UpdateQuestProgress("Explore", 1);
        }
    }
}
