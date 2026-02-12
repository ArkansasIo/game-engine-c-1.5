using UnityEngine;

namespace GoonZuInspired.DataTables
{
    // Placeholder for item data table (could be loaded from .csv/.json)
    [CreateAssetMenu(menuName = "GoonZuInspired/ItemTable")]
    public class ItemTable : ScriptableObject
    {
        public TextAsset CsvData;
        // Add parsing logic or strongly-typed fields as needed
    }
}
