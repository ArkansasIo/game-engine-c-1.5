using UnityEngine;

namespace GoonzuUI.Data.Skills
{
    [CreateAssetMenu(menuName = "GoonzuUI/SkillDef")]
    public sealed class SkillDef : ScriptableObject
    {
        public string skillId;
        public string displayName;
        public string description;
        public Sprite icon;
        public int maxLevel = 10;
    }
}
