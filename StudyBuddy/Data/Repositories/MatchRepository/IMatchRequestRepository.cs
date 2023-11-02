using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories.MatchRepository;

public interface IMatchRequestRepository
{
    Task<IEnumerable<MatchRequest>> GetAllAsync();
    Task<MatchRequest?> GetByIdAsync(int requestId);
    Task AddAsync(UserId currentUser, UserId otherUser);
    Task<bool> IsMatchRequestExistsAsync(UserId requesterId, UserId requestedId);
    Task DeleteAsync(UserId currentUser, UserId otherUser);
}
