using System;

namespace GoonzuGame.Multiplayer
{
    using System.Collections.Generic;

    public class Player
    {
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public Player(string name) { Name = name; IsOnline = true; }
    }

    public class Lobby
    {
        public List<Player> Players { get; set; }
        public Lobby() { Players = new List<Player>(); }
        public void AddPlayer(Player player)
        {
            Players.Add(player);
            Console.WriteLine($"Player {player.Name} joined the lobby.");
        }
        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
            Console.WriteLine($"Player {player.Name} left the lobby.");
        }
    }

    public class MultiplayerManager
    {
        public Lobby CurrentLobby { get; set; }
        public List<string> ChatLog { get; set; }

        public MultiplayerManager()
        {
            CurrentLobby = new Lobby();
            ChatLog = new List<string>();
        }

        public void CreateParty(string partyName) {
            System.Console.WriteLine($"Party '{partyName}' created.");
        }
        public void JoinParty(string player, string partyName) {
            System.Console.WriteLine($"{player} joined party '{partyName}'.");
        }

        public MultiplayerManager()
        {
            CurrentLobby = new Lobby();
            ChatLog = new List<string>();
        }

        public void CreateLobby()
        {
            CurrentLobby = new Lobby();
            Console.WriteLine("Lobby created.");
        }

        public void Matchmake(Player player)
        {
            CurrentLobby.AddPlayer(player);
            Console.WriteLine($"Matchmaking: {player.Name} added to lobby.");
        }

        public void SendChat(Player player, string message)
        {
            string chatMsg = $"{player.Name}: {message}";
            ChatLog.Add(chatMsg);
            Console.WriteLine(chatMsg);
        }

        public void SyncPlayers()
        {
            foreach (var player in CurrentLobby.Players)
            {
                Console.WriteLine($"Syncing player: {player.Name}");
            }
        }
    }
}
