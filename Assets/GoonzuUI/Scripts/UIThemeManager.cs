using UnityEngine;

namespace GoonzuUI
{
    [CreateAssetMenu(menuName = "GoonzuUI/UIThemeManager")]
    public sealed class UIThemeManager : ScriptableObject
    {
        public Color primaryColor = new Color(0.2f, 0.3f, 0.5f);
        public Color accentColor = new Color(0.8f, 0.7f, 0.2f);
        public Font font;
        // Add more style settings as needed
    }
}
