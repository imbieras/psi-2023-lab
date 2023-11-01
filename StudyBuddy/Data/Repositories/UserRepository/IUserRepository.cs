using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories.UserRepository;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(UserId userId);
    Task<List<string>?> GetHobbiesByIdAsync(UserId userId);
    Task AddHobbiesToUserAsync(User user, List<string> hobbies);
    Task UpdateAsync(User user, List<string>? updatedHobbies = null);
    Task DeleteAsync(UserId userId);
    Task<bool> UserExistsAsync(UserId userId);
    Task RemoveHobbyFromUserAsync(User user, string hobby);
}
