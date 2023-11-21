using Microsoft.EntityFrameworkCore;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories.MatchRepository;

public class MatchRequestRepository : IMatchRequestRepository
{
    private readonly StudyBuddyDbContext _context;
    private readonly ILogger<MatchRequestRepository> _logger;

    public MatchRequestRepository(StudyBuddyDbContext context, ILogger<MatchRequestRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<MatchRequest>> GetAllAsync() => await _context.MatchRequests.ToListAsync();

    public async Task AddAsync(UserId requesterId, UserId requestedId)
    {
        MatchRequest matchRequest = new(requesterId, requestedId);
        await _context.MatchRequests.AddAsync(matchRequest);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsMatchRequestExistsAsync(UserId requesterId, UserId requestedId) =>
        await _context.MatchRequests
            .AnyAsync(mr => mr.RequesterId == requesterId && mr.RequestedId == requestedId);

    public async Task RemoveAsync(UserId requesterId, UserId requestedId)
    {
        MatchRequest? matchRequest = await _context.MatchRequests
            .FirstOrDefaultAsync(mr => mr.RequesterId == requesterId && mr.RequestedId == requestedId);

        if (matchRequest != null)
        {
            _context.MatchRequests.Remove(matchRequest);
            await _context.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning($"Match request does not exist from {requesterId} to {requestedId}");
            throw new ArgumentException("Match request does not exist");
        }
    }
}
