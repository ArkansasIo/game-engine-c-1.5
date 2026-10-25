using UnityEngine;

namespace AIAssetTools
{
    public sealed class LinkedAssetReference : MonoBehaviour
    {
        [SerializeField] private string assetId;
        [SerializeField] private string generatedPath;
        [SerializeField] private string addressKey;
        [SerializeField] private int assetVersion = 1;

        public string AssetId => assetId;
        public string GeneratedPath => generatedPath;
        public string AddressKey => addressKey;
        public int AssetVersion => assetVersion;

        public void Bind(AssetEntry entry)
        {
            if (entry == null) return;
            assetId = entry.id;
            generatedPath = entry.generatedPath;
            addressKey = entry.addressKey;
            assetVersion = entry.version;
        }
    }
}
