using UnityEngine;

namespace GameEngine
{
    public sealed class GameEngine : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("GameEngine Awake: Initializing managers.");
            // Initialize managers (Scene, Resource, Audio, Physics, UI, etc.)
        }
        public void StartEngine()
        {
            Debug.Log("GameEngine started.");
        }
        public void UpdateEngine()
        {
            Debug.Log("GameEngine updated.");
        }
        public void ShutdownEngine()
        {
            Debug.Log("GameEngine shutdown.");
        }
    }
}
