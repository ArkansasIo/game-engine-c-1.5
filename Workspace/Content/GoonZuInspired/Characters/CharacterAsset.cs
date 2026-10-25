using UnityEngine;

namespace GoonZuInspired.Characters
{
    // Placeholder for character model, animation, texture, and skeleton references
    public class CharacterAsset : ScriptableObject
    {
        public string CharacterName;
        public GameObject BaseModel;
        public RuntimeAnimatorController AnimationController;
        public Texture2D[] Textures;
        public TextAsset SkeletonData;
        // Add more fields as needed for UE5/Unity asset integration
    }
}
