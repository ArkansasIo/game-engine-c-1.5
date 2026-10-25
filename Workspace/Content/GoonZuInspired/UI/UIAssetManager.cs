using System;
using System.Collections.Generic;

namespace GoonZuInspired.UI {
    public class UIAssetManager {
        // Asset storage
        private List<UIAsset> assets = new List<UIAsset>();

        // CRUD operations
        public void AddAsset(UIAsset asset) {
            assets.Add(asset);
        }

        public UIAsset GetAsset(string name) {
            return assets.Find(a => a.Name == name);
        }

        public bool UpdateAsset(string name, UIAsset updatedAsset) {
            var index = assets.FindIndex(a => a.Name == name);
            if (index >= 0) {
                assets[index] = updatedAsset;
                return true;
            }
            return false;
        }

        public bool DeleteAsset(string name) {
            return assets.RemoveAll(a => a.Name == name) > 0;
        }

        // Search/filter
        public List<UIAsset> SearchAssets(string keyword) {
            return assets.FindAll(a => a.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        // Import/export (stub)
        public void ImportAssets(string filePath) {
            // TODO: Implement import logic
        }

        public void ExportAssets(string filePath) {
            // TODO: Implement export logic
        }

        // Validation
        public bool ValidateAsset(UIAsset asset) {
            // TODO: Implement validation logic
            return !string.IsNullOrEmpty(asset.Name);
        }

        // Batch update
        public void BatchUpdate(Func<UIAsset, UIAsset> updateFunc) {
            for (int i = 0; i < assets.Count; i++) {
                assets[i] = updateFunc(assets[i]);
            }
        }
    }
}
