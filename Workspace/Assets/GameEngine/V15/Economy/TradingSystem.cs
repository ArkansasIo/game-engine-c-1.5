using System;
using System.Collections.Generic;

namespace GameEngine.V15.Economy
{
    public sealed record TradeOffer(string PlayerId, string ItemId, int Quantity, long Currency);

    public sealed class TradeSession
    {
        public string Id { get; }
        public string PlayerA { get; }
        public string PlayerB { get; }
        public TradeOffer? OfferA { get; private set; }
        public TradeOffer? OfferB { get; private set; }
        public bool LockedA { get; private set; }
        public bool LockedB { get; private set; }

        public TradeSession(string id, string playerA, string playerB)
        { Id = id; PlayerA = playerA; PlayerB = playerB; }

        public void SetOffer(TradeOffer offer)
        {
            if (offer.PlayerId == PlayerA) OfferA = offer;
            else if (offer.PlayerId == PlayerB) OfferB = offer;
            else throw new InvalidOperationException("Player is not part of this trade.");
        }
        public void Lock(string playerId)
        {
            if (playerId == PlayerA) LockedA = true;
            else if (playerId == PlayerB) LockedB = true;
        }
        public bool Ready => LockedA && LockedB;
    }

    public sealed class TradingSystem
    {
        private readonly Dictionary<string, TradeSession> sessions = new();
        public TradeSession Create(string id, string playerA, string playerB)
        {
            var session = new TradeSession(id, playerA, playerB);
            sessions.Add(id, session);
            return session;
        }
        public bool TryGet(string id, out TradeSession session) => sessions.TryGetValue(id, out session!);
        public bool Close(string id) => sessions.Remove(id);
    }
}
