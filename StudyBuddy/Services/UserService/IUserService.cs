using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services.UserService;

public interface IUserService
{
    Task<IUser?> GetUserByIdAsync(UserId userId);
    Task<IUser?> GetUserByUsernameAsync(string username);
    Task BanUserAsync(UserId userId);
    Task<UserId> RegisterUserAsync(string username, UserFlags flags, UserTraits traits);
    Task<IEnumerable<IUser>> GetAllUsersAsync();
    IUser? GetRandomUser(IUser user);
    IUser? GetPreviousRandomProfile(IUser user);
    IUser? GetCurrentRandomUser(IUser user);
    bool IsUsedIndexesEmpty(IUser user);
}
