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
            var player = new Character("Hero");
            Console.WriteLine($"Player {player.Name} initialized.");

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
            var quest = new Quest("First Steps");
            quest.Start();

            // Save/Load
            var saveLoad = new SaveLoadManager();
            saveLoad.SaveGame(player);
            saveLoad.LoadGame("save1");

            // UI
            var uiManager = new UIManager();
            uiManager.ShowMainMenu();
        }
    }
}
