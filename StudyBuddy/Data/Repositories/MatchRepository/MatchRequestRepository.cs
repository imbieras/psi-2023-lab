using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories;

using Microsoft.EntityFrameworkCore;
using StudyBuddy.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MatchRequestRepository : IMatchRequestRepository
{
    private readonly StudyBuddyDbContext _context;

    public MatchRequestRepository(StudyBuddyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MatchRequest>> GetAllAsync()
    {
        return await _context.MatchRequests.ToListAsync();
    }

    public async Task<MatchRequest?> GetByIdAsync(int requestId)
    {
        return await _context.MatchRequests.FindAsync(requestId);
    }

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
        var matchRequest = await _context.MatchRequests.FindAsync(requestId);
        if (matchRequest != null)
        {
            _context.MatchRequests.Remove(matchRequest);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsMatchRequestExistsAsync(UserId requesterId, UserId requestedId)
    {
        return await _context.MatchRequests
            .AnyAsync(mr => mr.RequesterId == requesterId && mr.RequestedId == requestedId);
    }

    public Task DeleteAsync(UserId currentUser, UserId otherUser)
    {
        var matchRequests = _context.MatchRequests
            .Where(mr => mr.RequesterId == currentUser && mr.RequestedId == otherUser
                         || mr.RequesterId == otherUser && mr.RequestedId == currentUser);
        _context.MatchRequests.RemoveRange(matchRequests);
        return Task.CompletedTask;
    }
}
