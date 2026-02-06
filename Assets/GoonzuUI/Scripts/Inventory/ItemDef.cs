using UnityEngine;

namespace GoonzuUI.Inventory
{
    [CreateAssetMenu(menuName = "GoonzuUI/ItemDef")]
    public sealed class ItemDef : ScriptableObject
    {
        public string itemId;
        public string displayName;
        public Sprite icon;
        public bool stackable = true;
        public int maxStack = 99;
    }
}
