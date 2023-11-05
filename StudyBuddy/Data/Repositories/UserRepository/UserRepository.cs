using Microsoft.EntityFrameworkCore;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories.UserRepository;

public class UserRepository : IUserRepository
{
    private readonly StudyBuddyDbContext _context;

    public UserRepository(StudyBuddyDbContext context) => _context = context;

    public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(UserId userId) => await _context.Users.FindAsync(userId);

    public async Task<List<string>?> GetHobbiesByIdAsync(UserId userId)
    {
        User? user = await _context.Users.FindAsync(userId);
        return user?.Hobbies;
    }

    public async Task UpdateAsync(User user, List<string>? updatedHobbies = null)
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
        User? user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UserExistsAsync(UserId userId) => await _context.Users.AnyAsync(u => u.Id == userId);

    public async Task AddHobbiesToUserAsync(User user, List<string> hobbies)
    {
        user.Hobbies ??= new List<string>();

        await _context.SaveChangesAsync();
    }

    public Task RemoveHobbyFromUserAsync(User user, string hobby)
    {
        user.Hobbies?.Remove(hobby);
        return Task.CompletedTask;
    }

    public Task<bool> IsUserSeenAsync(UserId userId, UserId otherUserId)
    {
        return _context.UserSeenProfiles.AnyAsync(u => u.UserId == userId && u.SeenUserId == otherUserId);
    }

    public async Task UserSeenAsync(UserId userId, UserId otherUserId)
    {
        _context.UserSeenProfiles.Add(new UserSeenProfile(userId, otherUserId));
        await _context.SaveChangesAsync();
    }

    public Task<bool> IsUserNotSeenAnyUserAsync(UserId userId)
    {
        return _context.UserSeenProfiles.AnyAsync(u => u.UserId == userId);
    }

    public Task<UserId> GetUltimateSeenUserAsync(UserId userId)
    {
        return _context.UserSeenProfiles
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.Timestamp)
            .Select(u => u.SeenUserId)
            .FirstOrDefaultAsync();
    }

    public Task<UserId> GetPenultimateSeenUserAsync(UserId userId)
    {
        return _context.UserSeenProfiles
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.Timestamp)
            .Select(u => u.SeenUserId)
            .Skip(1)
            .FirstOrDefaultAsync();
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
