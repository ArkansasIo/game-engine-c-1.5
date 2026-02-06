using UnityEngine;

namespace GoonzuUI.Data.Quests
{
    [CreateAssetMenu(menuName = "GoonzuUI/QuestDef")]
    public sealed class QuestDef : ScriptableObject
    {
        public string questId;
        public string title;
        public string description;
        public Sprite icon;
        // Add objectives, rewards, etc.
    }
}
