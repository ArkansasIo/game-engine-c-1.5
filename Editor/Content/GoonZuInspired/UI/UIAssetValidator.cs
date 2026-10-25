using System;

namespace GoonZuInspired.UI {
    public class UIAssetValidator {
        public static bool Validate(UIAsset asset) {
            if (string.IsNullOrEmpty(asset.Name) || string.IsNullOrEmpty(asset.Category) || string.IsNullOrEmpty(asset.FilePath))
                return false;
            if (asset.Level < 0)
                return false;
            if (string.IsNullOrEmpty(asset.Rarity))
                return false;
            return true;
        }
    }
}
