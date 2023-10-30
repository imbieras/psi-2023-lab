using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(UserId userId);
    Task<User?> GetWithHobbiesByIdAsync(UserId userId);
    Task AddAsync(User user, List<Hobby>? hobbies = null);
    Task UpdateAsync(User user, List<Hobby>? updatedHobbies = null);
    Task DeleteAsync(UserId userId);
    Task<bool> UserExistsAsync(UserId userId);
    Task AddHobbiesToUserAsync(User user, List<Hobby> hobbies);
    Task RemoveHobbyFromUserAsync(User user, Hobby hobby);
}

