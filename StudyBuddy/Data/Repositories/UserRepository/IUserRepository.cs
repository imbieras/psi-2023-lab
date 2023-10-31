using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(UserId userId);
    Task<List<Hobby>?> GetHobbiesByIdAsync(UserId userId);
    Task AddHobbiesToUserAsync(User user, List<Hobby> hobbies);
    Task UpdateAsync(User user, List<Hobby>? updatedHobbies = null);
    Task DeleteAsync(UserId userId);
    Task<bool> UserExistsAsync(UserId userId);
    Task RemoveHobbyFromUserAsync(User user, Hobby hobby);
}
