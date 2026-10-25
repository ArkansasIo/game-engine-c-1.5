using System;
using System.Collections.Generic;

namespace GameEngine.V15.Admin
{
    public sealed record AdminAuditEvent(string AdminId, string Action, string TargetId, DateTime Timestamp);

    public sealed class AdminService
    {
        private readonly HashSet<string> admins = new();
        private readonly List<AdminAuditEvent> audit = new();
        public IReadOnlyList<AdminAuditEvent> Audit => audit;
        public void Grant(string playerId) => admins.Add(playerId);
        public bool IsAdmin(string playerId) => admins.Contains(playerId);
        public void Record(string adminId, string action, string targetId)
        {
            if (!IsAdmin(adminId)) throw new UnauthorizedAccessException("Administrator permission required.");
            audit.Add(new AdminAuditEvent(adminId, action, targetId, DateTime.UtcNow));
        }
    }
}
