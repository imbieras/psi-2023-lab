using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services.UserService;

public interface IUserService
{
    Task<IEnumerable<IUser>> GetAllUsersAsync();
    Task<IUser?> GetUserByIdAsync(UserId userId);
    Task<List<string>?> GetHobbiesById(UserId userId);
    Task<IUser?> GetUserByUsernameAsync(string username);
    Task BanUserAsync(UserId userId);
    Task<UserId> RegisterUserAsync(string username, string password, UserFlags flags, UserTraits traits, List<string> hobbies);
    IUser? GetRandomUser();
    Task<IUser?> GetUltimateSeenUserAsync(UserId userId);
    Task<IUser?> GetPenultimateSeenUserAsync(UserId userId);
    Task<bool> IsUserNotSeenAnyUserAsync(UserId userId);
    Task<bool> IsUserSeenAsync(UserId userId, UserId otherUserId);
    Task UserSeenAsync(UserId userId, UserId otherUserId);
    Task AddHobbiesToUserAsync(User user, List<string> hobbies);
    Task AddHobbiesToUserAsync(UserId userid, List<string> hobbies);
}
