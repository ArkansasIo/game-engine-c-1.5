using UnityEngine;
using System.Collections.Generic;

namespace AIAssetTools
{
    /// <summary>
    /// Base class for managing assets generated from AI platforms
    /// </summary>
    public abstract class AIAssetManager : MonoBehaviour
    {
        [Header("AI Platform Settings")]
        public string platformName;
        public string apiKey; // For platforms that require API access
        
        [Header("Asset Organization")]
        public string assetFolderPath = "Assets/GoonzuGame/";
        public List<string> generatedAssets = new List<string>();
        
        protected virtual void Start()
        {
            InitializePlatform();
        }
        
        protected abstract void InitializePlatform();
        
        public virtual void ImportAsset(string assetPath)
        {
            if (!generatedAssets.Contains(assetPath))
            {
                generatedAssets.Add(assetPath);
                Debug.Log($"Imported {platformName} asset: {assetPath}");
            }
        }
        
        public virtual void OrganizeAssets()
        {
            // Implement asset organization logic
            Debug.Log($"Organizing assets from {platformName}");
        }
    }
}