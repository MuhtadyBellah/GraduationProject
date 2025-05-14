using System.Collections.Concurrent;

namespace ECommerce.ChatServices
{
    public static class connMapping
    {
        private static readonly ConcurrentDictionary<string, string> _connections = new();

        public static void Add(string userId, string connectionId)
            => _connections[userId] = connectionId;

        public static string? TryGetValue(string userId)
            => _connections.TryGetValue(userId, out string? connectionId) ? connectionId : null;

        public static void TryRemove(string userId)
            => _connections.TryRemove(userId, out _);
    }
}
