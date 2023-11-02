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

    public async Task AddAsync(UserId requesterId, UserId requestedId)
    {
        MatchRequest matchRequest = new(requesterId, requestedId);
        await _context.MatchRequests.AddAsync(matchRequest);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsMatchRequestExistsAsync(UserId requesterId, UserId requestedId) =>
        await _context.MatchRequests
            .AnyAsync(mr => mr.RequesterId == requesterId && mr.RequestedId == requestedId);

    public async Task DeleteAsync(UserId requesterId, UserId requestedId)
    {
        _context.MatchRequests.Remove(new MatchRequest(requesterId, requestedId));
        await _context.SaveChangesAsync();
    }
}
