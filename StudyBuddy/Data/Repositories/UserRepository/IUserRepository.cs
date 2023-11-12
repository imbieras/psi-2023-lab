using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories.UserRepository;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(UserId userId);
    Task<List<string>?> GetHobbiesByIdAsync(UserId userId);
    Task UpdateHobbiesAsync(UserId userId, List<string>? updatedHobbies);
    Task DeleteAsync(UserId userId);
    Task<bool> UserExistsAsync(UserId userId);
    Task<bool> IsUserSeenAsync(UserId userId, UserId otherUserId);
    Task UserSeenAsync(UserId userId, UserId otherUserId);
    Task<bool> IsUserNotSeenAnyUserAsync(UserId userId);
    Task<UserId> GetUltimateSeenUserAsync(UserId userId);
    Task<UserId> GetPenultimateSeenUserAsync(UserId userId);
    Task UpdateAsync(User user);
}
