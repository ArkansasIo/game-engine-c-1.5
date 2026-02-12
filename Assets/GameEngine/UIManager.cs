using UnityEngine;

namespace GameEngine
{
    public sealed class UIManager : MonoBehaviour
    {
        public void InitUI()
        {
            Debug.Log("UIManager initialized.");
        }
        public void UpdateUI()
        {
            Debug.Log("UIManager updated.");
        }
        public void ShutdownUI()
        {
            Debug.Log("UIManager shutdown.");
        }
    }
}
