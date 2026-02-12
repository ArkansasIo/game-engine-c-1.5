using UnityEngine;
using UnityEditor;

namespace GoonZuInspired.Items
{
    // Editor utility for importing item models, icons, and data
    public class ItemAssetImporter : EditorWindow
    {
        [MenuItem("GoonZuInspired/Import/Item Asset")]
        public static void ShowWindow()
        {
            GetWindow<ItemAssetImporter>("Item Asset Importer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Import Item Asset", EditorStyles.boldLabel);
            // Add fields for model, icon, data, etc.
            // Implement import logic as needed
        }
    }
}
