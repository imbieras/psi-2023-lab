using Microsoft.EntityFrameworkCore;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Data.Repositories.UserRepository;

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

    public async Task UserSeenAsync(UserId userId, UserId otherUserId)
    {
        _context.UserSeenProfiles.Add(new UserSeenProfile(userId, otherUserId));
        await _context.SaveChangesAsync();
    }

    public Task<bool> IsUserSeenAsync(UserId userId, UserId otherUserId) =>
        _context.UserSeenProfiles.AnyAsync(u => u.UserId == userId && u.SeenUserId == otherUserId);

    public async Task<bool> IsUserNotSeenAnyUserAsync(UserId userId)
    {
        return !await _context.UserSeenProfiles.AnyAsync(u => u.UserId == userId);
    }

    public Task<UserId> GetUltimateSeenUserAsync(UserId userId) =>
        _context.UserSeenProfiles
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.Timestamp)
            .Select(u => u.SeenUserId)
            .FirstOrDefaultAsync();

    public Task<UserId> GetPenultimateSeenUserAsync(UserId userId) =>
        _context.UserSeenProfiles
            .Where(u => u.UserId == userId)
            .OrderByDescending(u => u.Timestamp)
            .Select(u => u.SeenUserId)
            .Skip(1)
            .FirstOrDefaultAsync();

    public async Task UpdateAsync(User user)
    {
        var local = _context.Set<User>()
            .Local
            .FirstOrDefault(entry => entry.Id.Equals(user.Id));

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        _context.Entry(user).State = EntityState.Modified;
        await _context.SaveChangesAsync();
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
