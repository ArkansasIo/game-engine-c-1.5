using System;

namespace GoonzuGame.WorldMap
{
    using System.Collections.Generic;

    public class WorldMapManager
    {
        public List<string> Locations { get; set; }
        public List<string> Markers { get; set; }
        public string CurrentLocation { get; set; }

        public WorldMapManager()
        {
            Locations = new List<string> { "Town", "Forest", "Dungeon", "Market" };
            Markers = new List<string>();
            CurrentLocation = "Town";
        }

        public void ShowMap()
        {
            Console.WriteLine("World Map:");
            foreach (var loc in Locations)
                Console.WriteLine($"- {loc}");
        }

        public void FastTravel(string location)
        {
            if (Locations.Contains(location))
            {
                CurrentLocation = location;
                Console.WriteLine($"Fast traveled to: {location}");
            }
            else
            {
                Console.WriteLine($"Location '{location}' not found.");
            }
        }

        public void AddMarker(string markerName)
        {
            Markers.Add(markerName);
            Console.WriteLine($"Added map marker: {markerName}");
        }
    }
}
