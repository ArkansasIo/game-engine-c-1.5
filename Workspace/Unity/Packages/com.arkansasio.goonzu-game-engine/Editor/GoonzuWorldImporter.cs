using UnityEditor;
using UnityEngine;
using ArkansasIo.GoonzuGameEngine;

namespace ArkansasIo.GoonzuGameEngine.Editor;

public static class GoonzuWorldImporter
{
    [MenuItem("Assets/Goonzu/Create World Definition From JSON", true)]
    private static bool ValidateCreateWorldDefinition()
    {
        return Selection.activeObject is TextAsset text && AssetDatabase.GetAssetPath(text).EndsWith(".json", System.StringComparison.OrdinalIgnoreCase);
    }

    [MenuItem("Assets/Goonzu/Create World Definition From JSON")]
    private static void CreateWorldDefinition()
    {
        var json = Selection.activeObject as TextAsset;
        if (json == null) return;

        var sourcePath = AssetDatabase.GetAssetPath(json);
        var definition = GoonzuWorldLoader.LoadJson(json);
        var destination = System.IO.Path.ChangeExtension(sourcePath, ".asset");
        destination = AssetDatabase.GenerateUniqueAssetPath(destination);
        AssetDatabase.CreateAsset(definition, destination);
        AssetDatabase.SaveAssets();
        Selection.activeObject = definition;
        EditorGUIUtility.PingObject(definition);
    }
}
