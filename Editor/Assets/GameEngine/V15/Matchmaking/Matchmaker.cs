using System;
using System.Collections.Generic;
using System.Linq;

namespace GameEngine.V15.Matchmaking
{
    public sealed record QueueEntry(string PlayerId, int Rating, DateTime EnqueuedAt);

    public sealed class Matchmaker
    {
        private readonly List<QueueEntry> queue = new();
        public void Enqueue(string playerId, int rating) => queue.Add(new QueueEntry(playerId, rating, DateTime.UtcNow));
        public bool Remove(string playerId) => queue.RemoveAll(x => x.PlayerId == playerId) > 0;

        public IReadOnlyList<(QueueEntry A, QueueEntry B)> BuildMatches(int ratingWindow = 200)
        {
            var result = new List<(QueueEntry, QueueEntry)>();
            var ordered = queue.OrderBy(x => x.EnqueuedAt).ToList();
            var used = new HashSet<string>();
            for (var i = 0; i < ordered.Count; i++)
            {
                if (used.Contains(ordered[i].PlayerId)) continue;
                for (var j = i + 1; j < ordered.Count; j++)
                {
                    if (used.Contains(ordered[j].PlayerId)) continue;
                    if (Math.Abs(ordered[i].Rating - ordered[j].Rating) <= ratingWindow)
                    {
                        result.Add((ordered[i], ordered[j]));
                        used.Add(ordered[i].PlayerId);
                        used.Add(ordered[j].PlayerId);
                        break;
                    }
                }
            }
            return result;
        }
    }
}
