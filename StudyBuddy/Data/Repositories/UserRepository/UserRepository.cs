using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using StudyBuddy.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserRepository : IUserRepository
{
    private readonly StudyBuddyDbContext _context;

    public UserRepository(StudyBuddyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(UserId userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<List<Hobby>?> GetHobbiesByIdAsync(UserId userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user?.Hobbies;
    }

    public async Task UpdateAsync(User user, List<Hobby>? updatedHobbies = null)
    {
        _context.Users.Update(user);

        if (updatedHobbies != null)
        {
            // Remove all hobbies from the user
            user.Hobbies?.Clear();
            await AddHobbiesToUserAsync(user, updatedHobbies);
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(UserId userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UserExistsAsync(UserId userId)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId);
    }

    public async Task AddHobbiesToUserAsync(User user, List<Hobby> hobbies)
    {
        // Hobbies are stored serialized in the User object, so we need to add it there
        user.Hobbies ??= new List<Hobby>();

        await _context.SaveChangesAsync();
    }

    public Task RemoveHobbyFromUserAsync(User user, Hobby hobby)
    {
        // Hobbies are stored serialized in the User object, so we need to remove it from there
        user.Hobbies?.Remove(hobby);
        return Task.CompletedTask;
    }

    public async Task AddAsync(User user)
    {
        if (!await UserExistsAsync(user.Id))
        {
            _context.Users.Add(user);
        }
        await _context.SaveChangesAsync();
    }
}
