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

    public async Task AddAsync(Match match)
    {
        await _context.Matches.AddAsync(match);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Match match)
    {
        _context.Matches.Update(match);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int matchId)
    {
        Match? match = await _context.Matches.FindAsync(matchId);
        if (match != null)
        {
            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();
        }
    }

    public Task<bool> IsMatchAsync(UserId currentUser, UserId otherUser)
    {
        // Bigger user id is always user1
        UserId user1 = currentUser.CompareTo(otherUser) < 0 ? currentUser : otherUser;
        UserId user2 = currentUser.CompareTo(otherUser) < 0 ? otherUser : currentUser;

        return _context.Matches
            .AnyAsync(m => m.User1Id == user1 && m.User2Id == user2);
    }

    public Task<List<Match>> GetMatchHistoryByUserIdAsync(UserId userId) =>
        _context.Matches
            .Where(m => m.User1Id == userId || m.User2Id == userId)
            .ToListAsync();
}
