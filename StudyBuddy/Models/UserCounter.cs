using System.Collections.Concurrent;
using StudyBuddy.Abstractions;
using StudyBuddy.Services.UserService;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public static class UserCounter
{
    private static readonly ConcurrentBag<UserId> s_activeUsers = new();

    public static int TotalUsers => s_activeUsers.Count;

    public static void AddUser(UserId userId) => s_activeUsers.Add(userId);

    public static void RemoveUser(UserId userId) => s_activeUsers.TryTake(out userId);

    public static async Task InitializeAsync(IUserService userService)
    {
        IEnumerable<IUser> allUsers = await userService.GetAllUsersAsync();// Assuming this method exists and returns all users
        foreach (IUser? user in allUsers)
        {
            s_activeUsers.Add(user.Id);
        }
    }
}
