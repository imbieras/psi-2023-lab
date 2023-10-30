using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories;

public interface IMatchRequestRepository
{
    Task<IEnumerable<MatchRequest>> GetAllAsync();
    Task<MatchRequest?> GetByIdAsync(int requestId);
    Task AddAsync(MatchRequest matchRequest);
    Task UpdateAsync(MatchRequest matchRequest);
    Task DeleteAsync(int requestId);
    Task<bool> IsMatchRequestExistsAsync(UserId requesterId, UserId requestedId);
    Task DeleteAsync(UserId currentUser, UserId otherUser);
}
