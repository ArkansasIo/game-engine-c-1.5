using System;
using System.Collections.Generic;

namespace GoonZuInspired.UI {
    public class UIAssetLinker {
        private readonly Dictionary<string, List<string>> links = new();

        public void Link(string assetName, string relatedEntity) {
            if (!links.ContainsKey(assetName))
                links[assetName] = new List<string>();
            if (!string.IsNullOrEmpty(relatedEntity) && !links[assetName].Contains(relatedEntity))
                links[assetName].Add(relatedEntity);
        }

        public IReadOnlyList<string> GetLinks(string assetName) {
            if (links.ContainsKey(assetName))
                return links[assetName].AsReadOnly();
            return new List<string>().AsReadOnly();
        }
    }
}
