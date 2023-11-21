using System.Collections.Concurrent;

namespace StudyBuddy.Models;

public static class UserCounter
{
    private static readonly ConcurrentDictionary<Guid, bool> s_activeUsers = new ConcurrentDictionary<Guid, bool>();

    public static int TotalUsers => s_activeUsers.Count;

    public static void AddUser(Guid userId)
    {
        s_activeUsers.TryAdd(userId, true);
    }

    public static void RemoveUser(Guid userId)
    {
        bool removed;
        s_activeUsers.TryRemove(userId, out removed);
    }
}
