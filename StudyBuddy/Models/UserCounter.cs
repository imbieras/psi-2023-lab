using System.Collections.Concurrent;
using StudyBuddy.Services.UserService;
using System.Threading.Tasks;

namespace StudyBuddy.Models
{
    public static class UserCounter
    {
        private static readonly ConcurrentDictionary<Guid, bool> ActiveUsers = new ConcurrentDictionary<Guid, bool>();

        public static int TotalUsers => ActiveUsers.Count;

        public static void AddUser(Guid userId)
        {
            ActiveUsers.TryAdd(userId, true);
        }

        public static void RemoveUser(Guid userId)
        {
            ActiveUsers.TryRemove(userId, out bool _);
        }

        public static async Task InitializeAsync(IUserService userService)
        {
            var allUsers = await userService.GetAllUsersAsync(); // Assuming this method exists and returns all users
            foreach (var user in allUsers)
            {
                ActiveUsers.TryAdd((Guid)user.Id, true);
            }
        }
    }
}
