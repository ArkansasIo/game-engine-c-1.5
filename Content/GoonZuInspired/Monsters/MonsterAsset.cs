using UnityEngine;

namespace GoonZuInspired.Monsters
{
    // Placeholder for monster model, animation, and texture references
    public class MonsterAsset : ScriptableObject
    {
        public string MonsterName;
        public GameObject Model;
        public RuntimeAnimatorController AnimationController;
        public Texture2D[] Textures;
        // Add more fields as needed
    }
}
