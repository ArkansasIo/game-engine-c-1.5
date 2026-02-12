using UnityEngine;
using UnityEditor;

namespace GoonZuInspired.DataTables
{
    // Editor utility for importing data tables (CSV, JSON, etc.)
    public class DataTableImporter : EditorWindow
    {
        [MenuItem("GoonZuInspired/Import/Data Table")]
        public static void ShowWindow()
        {
            GetWindow<DataTableImporter>("Data Table Importer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Import Data Table", EditorStyles.boldLabel);
            // Add fields for CSV/JSON file selection
            // Implement import logic as needed
        }
    }
}
