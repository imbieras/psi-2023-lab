using StudyBuddy.Shared.DTOs;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Data.Repositories.UserRepository;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(UserId userId);
    Task<List<string>?> GetHobbiesByIdAsync(UserId userId);
    Task DeleteAsync(UserId userId);
    Task<bool> UserExistsAsync(UserId userId);
    Task<bool> IsUserSeenAsync(UserId userId, UserId otherUserId);
    Task UserSeenAsync(UserId userId, UserId otherUserId);
    Task<bool> IsUserNotSeenAnyUserAsync(UserId userId);
    Task<UserId> GetUltimateSeenUserAsync(UserId userId);
    Task<UserId> GetPenultimateSeenUserAsync(UserId userId);
    Task UpdateAsync(User user);
    Task UpdateAsync(UserId userId, UpdateUserDto updateUserDto);
}
