using UnityEngine;
using UnityEditor;

namespace GoonZuInspired.Monsters
{
    // Editor utility for importing monster models, textures, and animations
    public class MonsterAssetImporter : EditorWindow
    {
        [MenuItem("GoonZuInspired/Import/Monster Asset")]
        public static void ShowWindow()
        {
            GetWindow<MonsterAssetImporter>("Monster Asset Importer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Import Monster Asset", EditorStyles.boldLabel);
            // Add fields for model, textures, animation, etc.
            // Implement import logic as needed
        }
    }
}
