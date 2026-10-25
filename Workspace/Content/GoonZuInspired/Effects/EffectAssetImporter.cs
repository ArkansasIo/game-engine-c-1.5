using UnityEngine;
using UnityEditor;

namespace GoonZuInspired.Effects
{
    // Editor utility for importing effect prefabs and data
    public class EffectAssetImporter : EditorWindow
    {
        [MenuItem("GoonZuInspired/Import/Effect Asset")]
        public static void ShowWindow()
        {
            GetWindow<EffectAssetImporter>("Effect Asset Importer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Import Effect Asset", EditorStyles.boldLabel);
            // Add fields for effect prefab, data, etc.
            // Implement import logic as needed
        }
    }
}
