using UnityEngine;

namespace GameEngine
{
    public sealed class GameEngine : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("GameEngine Awake: Initializing managers.");
            // Initialize managers (Scene, Resource, Audio, Physics, UI, etc.)
            InitializeManagers();
        }

        private void InitializeManagers()
        {
            // Ensure all managers are instantiated
            var achievementManager = GoonzuGame.Achievements.AchievementManager.Instance;
            var inventoryManager = GoonzuGame.Inventory.InventoryManager.Instance;
            var questManager = GoonzuGame.Quests.QuestManager.Instance;
            var skillTreeManager = GoonzuGame.Skills.SkillTreeManager.Instance;
            var aiManager = GoonzuGame.AI.AIManager.Instance;
            var saveLoadManager = GoonzuGame.SaveLoad.SaveLoadManager.Instance;
            var uiManager = GoonzuGame.UI.UIManager.Instance;
            var mapManager = GoonzuGame.Map.MapManager.Instance;
            var tradeManager = GoonzuGame.Trade.TradeManager.Instance;
            var combatSystem = GoonzuGame.Combat.CombatSystem.Instance;
            var craftingManager = GoonzuGame.Crafting.CraftingManager.Instance;
            var professionManager = GoonzuGame.Professions.ProfessionManager.Instance;
            var guildManager = GoonzuGame.Guilds.GuildManager.Instance;
            var multiplayerManager = GoonzuGame.Multiplayer.MultiplayerManager.Instance;
            var monsterManager = GoonzuGame.Monsters.MonsterManager.Instance;
            var npcManager = GoonzuGame.NPC.NPCManager.Instance;

            Debug.Log("All managers initialized.");
        }

        public void StartEngine()
        {
            Debug.Log("GameEngine started.");
            // Load initial scene or start game
            GoonzuGame.Map.MapManager.Instance.LoadMap("Town");
        }

        public void UpdateEngine()
        {
            Debug.Log("GameEngine updated.");
            // Update player UI
            var player = FindObjectOfType<GoonzuGame.Characters.Character>();
            if (player != null)
            {
                GoonzuGame.UI.UIManager.Instance.UpdatePlayerUI(player);
            }
        }

        public void ShutdownEngine()
        {
            Debug.Log("GameEngine shutdown.");
            // Save game
            GoonzuGame.SaveLoad.SaveLoadManager.Instance.AutoSave();
        }
    }
}
