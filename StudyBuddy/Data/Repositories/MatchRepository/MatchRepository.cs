using StudyBuddy.ValueObjects;
using Microsoft.EntityFrameworkCore;
using StudyBuddy.Models;

namespace StudyBuddy.Data.Repositories.MatchRepository;

public class MatchRepository : IMatchRepository
{
    private readonly StudyBuddyDbContext _context;

    public MatchRepository(StudyBuddyDbContext context) => _context = context;

    public async Task<IEnumerable<Match>> GetAllAsync() => await _context.Matches.ToListAsync();

    public async Task<Match?> GetByIdAsync(int matchId) => await _context.Matches.FindAsync(matchId);

    public async Task AddAsync(UserId user1Id, UserId user2Id)
    {
        await _context.Matches.AddAsync(new Match(user1Id, user2Id));
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAsync(UserId user1Id, UserId user2Id)
    {
        UserId user1 = user1Id.CompareTo(user2Id) < 0 ? user1Id : user2Id;
        UserId user2 = user1Id.CompareTo(user2Id) < 0 ? user2Id : user1Id;

        Match? match = await _context.Matches
            .FirstOrDefaultAsync(m => m.User1Id == user1 && m.User2Id == user2);

        if (match != null)
        {
            _context.Matches.Remove(match);
        }
        else
        {
            throw new ArgumentException("Match does not exist");
        }
    }

    public async Task<bool> IsMatchAsync(UserId currentUser, UserId otherUser)
    {
        UserId user1 = currentUser.CompareTo(otherUser) < 0 ? currentUser : otherUser;
        UserId user2 = currentUser.CompareTo(otherUser) < 0 ? otherUser : currentUser;

        return await _context.Matches
            .AnyAsync(m => m.User1Id == user1 && m.User2Id == user2);
    }

    public Task<List<Match>> GetMatchHistoryByUserIdAsync(UserId userId) =>
        _context.Matches
            .Where(m => m.User1Id == userId || m.User2Id == userId)
            .ToListAsync();
}
