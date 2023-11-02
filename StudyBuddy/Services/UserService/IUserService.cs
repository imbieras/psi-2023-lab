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
    Task<UserId> RegisterUserAsync(string username, UserFlags flags, UserTraits traits, List<string> hobbies);
    IUser? GetRandomUser(IUser user);
    IUser? GetPreviousRandomProfile(IUser user);
    IUser? GetCurrentRandomUser(IUser user);
    bool IsUsedIndexesEmpty(IUser user);
    Task AddHobbiesToUserAsync(User user, List<string> hobbies);
    Task AddHobbiesToUserAsync(UserId userid, List<string> hobbies);

}
