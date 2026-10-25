using System;
using System.Collections.Generic;

namespace GameEngine.V15.Instances
{
    public enum InstanceState { Waiting, Active, Completed, Failed }

    public sealed class DungeonInstance
    {
        private readonly HashSet<string> players = new();
        public string Id { get; }
        public int Difficulty { get; }
        public InstanceState State { get; private set; } = InstanceState.Waiting;
        public IReadOnlyCollection<string> Players => players;

        public DungeonInstance(string id, int difficulty)
        {
            Id = id;
            Difficulty = Math.Max(1, difficulty);
        }
        public bool AddPlayer(string playerId) => players.Add(playerId);
        public bool RemovePlayer(string playerId) => players.Remove(playerId);
        public void Start() { if (players.Count == 0) throw new InvalidOperationException("Cannot start an empty instance."); State = InstanceState.Active; }
        public void Complete() { if (State == InstanceState.Active) State = InstanceState.Completed; }
        public void Fail() { if (State == InstanceState.Active) State = InstanceState.Failed; }
    }
}
