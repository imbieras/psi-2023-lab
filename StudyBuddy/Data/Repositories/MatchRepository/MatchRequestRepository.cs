using Microsoft.EntityFrameworkCore;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories.MatchRepository;

public class MatchRequestRepository : IMatchRequestRepository
{
    private readonly StudyBuddyDbContext _context;

    public MatchRequestRepository(StudyBuddyDbContext context) => _context = context;

    public async Task<IEnumerable<MatchRequest>> GetAllAsync() => await _context.MatchRequests.ToListAsync();

    public async Task<MatchRequest?> GetByIdAsync(int requestId) => await _context.MatchRequests.FindAsync(requestId);

    public async Task AddAsync(MatchRequest matchRequest)
    {
        await _context.MatchRequests.AddAsync(matchRequest);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MatchRequest matchRequest)
    {
        _context.MatchRequests.Update(matchRequest);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int requestId)
    {
        MatchRequest? matchRequest = await _context.MatchRequests.FindAsync(requestId);
        if (matchRequest != null)
        {
            _context.MatchRequests.Remove(matchRequest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsMatchRequestExistsAsync(UserId requesterId, UserId requestedId) =>
        await _context.MatchRequests
            .AnyAsync(mr => mr.RequesterId == requesterId && mr.RequestedId == requestedId);

    public Task DeleteAsync(UserId currentUser, UserId otherUser)
    {
        IQueryable<MatchRequest> matchRequests = _context.MatchRequests
            .Where(mr => (mr.RequesterId == currentUser && mr.RequestedId == otherUser)
                         || (mr.RequesterId == otherUser && mr.RequestedId == currentUser));
        _context.MatchRequests.RemoveRange(matchRequests);
        return Task.CompletedTask;
    }
}
