using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Data.Repositories.MatchRepository;

public interface IMatchRequestRepository
{
    Task<IEnumerable<MatchRequest>> GetAllAsync();
    Task AddAsync(UserId currentUser, UserId otherUser);
    Task<bool> IsMatchRequestExistsAsync(UserId requesterId, UserId requestedId);
    Task RemoveAsync(UserId currentUser, UserId otherUser);
}
