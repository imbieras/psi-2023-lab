using System.Collections.Concurrent;
using StudyBuddy.API.Services;
using StudyBuddy.API.Services.UserService;
using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Models;

public static class UserCounter
{
    private static readonly ConcurrentBag<UserId> s_activeUsers = new();

    public static int TotalUsers => s_activeUsers.Count;

    public static void AddUser(UserId userId) => s_activeUsers.Add(userId);

    public static void RemoveUser(UserId userId) => s_activeUsers.TryTake(out userId);

    public static async Task InitializeAsync(IUserService userService)
    {
        IEnumerable<IUser> allUsers = await userService.GetAllUsersAsync();

        UserPredicate isActiveUserDelegate = user =>
            (user.Flags & UserFlags.Banned) == 0 && (user.Flags & UserFlags.Admin) == 0;

        List<IUser>? activeUsers = GenericFilterService<IUser>.FilterUsersByPredicate(allUsers, isActiveUserDelegate);

        foreach (IUser user in activeUsers)
        {
            s_activeUsers.Add(user.Id);
        }
    }
}
