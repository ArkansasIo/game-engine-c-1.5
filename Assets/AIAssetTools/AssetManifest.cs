using System;
using System.Collections.Generic;
using UnityEngine;

namespace AIAssetTools
{
    [CreateAssetMenu(fileName = "GoonzuAssetManifest", menuName = "Goonzu/Asset Manifest")]
    public sealed class AssetManifest : ScriptableObject
    {
        public string manifestVersion = "2.0";
        public List<AssetEntry> assets = new List<AssetEntry>();

        public bool TryGet(string id, out AssetEntry entry)
        {
            entry = assets.Find(x => string.Equals(x.id, id, StringComparison.OrdinalIgnoreCase));
            return entry != null;
        }
    }

    [Serializable]
    public sealed class AssetEntry
    {
        public string id;
        public string category;
        public string sourcePath;
        public string generatedPath;
        public string prompt;
        public string assetType = "Sprite";
        public string addressKey;
        public int version = 1;
        public bool generated;
        public bool imported;
    }
}
