
using System.Collections.Generic;

namespace GoonzuGame.Multiplayer
{
    [System.Serializable]
    public class Player
    {
        public string Name;
        public bool IsOnline;
        public GoonzuGame.Characters.Character Character;

        public Player(string name, GoonzuGame.Characters.Character character)
        {
            Name = name;
            IsOnline = true;
            Character = character;
        }
    }

    [System.Serializable]
    public class Lobby
    {
        public string Name;
        public List<Player> Players = new List<Player>();
        public int MaxPlayers = 4;

        public Lobby(string name)
        {
            Name = name;
        }

        public bool AddPlayer(Player player)
        {
            if (Players.Count < MaxPlayers)
            {
                Players.Add(player);
                System.Console.WriteLine($"Player {player.Name} joined the lobby {Name}.");
                return true;
            }
            return false;
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
            System.Console.WriteLine($"Player {player.Name} left the lobby {Name}.");
        }
    }

    public class MultiplayerManager
    {
        public static MultiplayerManager Instance { get; private set; }

        public Lobby CurrentLobby;
        public List<string> ChatLog = new List<string>();
        public bool IsConnected;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void Connect()
        {
            IsConnected = true;
            System.Console.WriteLine("Multiplayer connected.");
            // In real implementation, connect to server
        }

        public void Disconnect()
        {
            IsConnected = false;
            System.Console.WriteLine("Multiplayer disconnected.");
        }

        public void CreateLobby(string lobbyName)
        {
            CurrentLobby = new Lobby(lobbyName);
            System.Console.WriteLine($"Lobby '{lobbyName}' created.");
        }

        public void JoinLobby(string lobbyName)
        {
            // In real implementation, find and join lobby
            if (CurrentLobby == null)
            {
                CurrentLobby = new Lobby(lobbyName);
            }
            System.Console.WriteLine($"Joined lobby '{lobbyName}'.");
        }

        public void LeaveLobby()
        {
            if (CurrentLobby != null)
            {
                CurrentLobby = null;
                System.Console.WriteLine("Left lobby.");
            }
        }

        public void Matchmake(Player player)
        {
            if (CurrentLobby != null)
            {
                CurrentLobby.AddPlayer(player);
            }
        }

        public void SendChat(Player player, string message)
        {
            if (!IsConnected) return;

            string chatMsg = $"{player.Name}: {message}";
            ChatLog.Add(chatMsg);
            System.Console.WriteLine(chatMsg);
            // Broadcast to other players
        }

        public void SyncPlayers()
        {
            if (CurrentLobby != null)
            {
                foreach (var player in CurrentLobby.Players)
                {
                    System.Console.WriteLine($"Syncing player: {player.Name}");
                    // Sync position, stats, etc.
                }
            }
        }

        public void CreateParty(string partyName)
        {
            System.Console.WriteLine($"Party '{partyName}' created.");
            // Similar to lobby but for smaller groups
        }

        public void JoinParty(string playerName, string partyName)
        {
            System.Console.WriteLine($"{playerName} joined party '{partyName}'.");
        }

        public List<string> GetChatLog()
        {
            return ChatLog;
        }

        public List<Player> GetPlayersInLobby()
        {
            return CurrentLobby?.Players ?? new List<Player>();
        }
    }
}
