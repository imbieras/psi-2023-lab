using Microsoft.EntityFrameworkCore;
using StudyBuddy.Models;
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

    public async Task<User?> GetWithHobbiesByIdAsync(UserId userId)
    {
        return await _context.Users
            .Include(u => u.UserHobbies)
            .ThenInclude(uh => uh.Hobby)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task AddAsync(User user, List<Hobby>? hobbies = null)
    {
        await _context.Users.AddAsync(user);

        if (hobbies != null)
        {
            foreach (var hobby in hobbies)
            {
                await _context.UserHobbies.AddAsync(new UserHobby { UserId = user.Id, HobbyId = hobby.Id });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user, List<Hobby>? updatedHobbies = null)
    {
        _context.Users.Update(user);

        if (updatedHobbies != null)
        {
            // Remove old hobbies
            var existingHobbies = await _context.UserHobbies
                .Where(uh => uh.UserId == user.Id)
                .ToListAsync();
            _context.UserHobbies.RemoveRange(existingHobbies);

            // Add updated hobbies
            foreach (var hobby in updatedHobbies)
            {
                await _context.UserHobbies.AddAsync(new UserHobby (user.Id, user, hobby.Id, hobby));
            }
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
        foreach (var userHobby in hobbies.Select(hobby => new UserHobby(user.Id, user, hobby.Id, hobby)))
        {
            await _context.UserHobbies.AddAsync(userHobby);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveHobbyFromUserAsync(User user, Hobby hobby)
    {
        var userHobby = await _context.UserHobbies
            .Include(uh => uh.User)
            .Include(uh => uh.Hobby)
            .FirstOrDefaultAsync(uh => uh.UserId == user.Id && uh.HobbyId == hobby.Id);

        if (userHobby != null)
        {
            _context.UserHobbies.Remove(userHobby);
            await _context.SaveChangesAsync();
        }
    }
}
