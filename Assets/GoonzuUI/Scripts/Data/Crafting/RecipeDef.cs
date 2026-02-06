using UnityEngine;

namespace GoonzuUI.Data.Crafting
{
    [CreateAssetMenu(menuName = "GoonzuUI/RecipeDef")]
    public sealed class RecipeDef : ScriptableObject
    {
        public string recipeId;
        public string displayName;
        public Sprite icon;
        // Add required materials, output item, etc.
    }
}
