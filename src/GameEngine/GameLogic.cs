using GameEngine.AI;
using GameEngine.Dialogue;
using GoonzuGame.World;
using GoonzuGame.Network;
using GoonzuGame.Multiplayer;
using GoonzuGame.Characters;
using GoonzuGame.Quests;
using GoonzuGame.SaveLoad;
using GoonzuGame.UI;
using System;

namespace GameEngine {
    public class GameLogic {
        public void Run() {
            // Initialize world
            var world = new GoonzuWorld();
            world.RenderWorld();

            // Initialize player
            var player = new Character("Hero", GoonzuGame.Characters.CharacterClass.Warrior);

            // Initialize AI
            var aiManager = new AIManager();
            aiManager.RegisterAgent("Goblin");
            aiManager.UpdateAgents();

            // Initialize Dialogue
            var dialogue = new DialogueManager();
            dialogue.Initialize();
            dialogue.StartDialogue("Guard");
            dialogue.Next();
            dialogue.EndDialogue();

            // Initialize Multiplayer
            var multiplayer = new MultiplayerManager();
            multiplayer.Connect();

            // Initialize Quests
            var quest = new Quest("First Steps", "Complete the initial training");
            quest.StartQuest();

            // Save/Load
            var saveLoad = SaveLoadManager.Instance;
            saveLoad.SaveGame(player);
            saveLoad.LoadGame("save1", player);

            // UI
            Console.WriteLine("GameLogic demo complete.");
        }
    }
}
