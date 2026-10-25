using UnityEngine;

namespace GameEngine
{
    public sealed class ResourceManager : MonoBehaviour
    {
        public void InitResources()
        {
            Debug.Log("ResourceManager initialized.");
        }
        public void LoadResource(string resource)
        {
            Debug.Log($"Loading resource: {resource}");
        }
        public void UnloadResource(string resource)
        {
            Debug.Log($"Unloading resource: {resource}");
        }
        public void ShutdownResources()
        {
            Debug.Log("ResourceManager shutdown.");
        }
    }
}
