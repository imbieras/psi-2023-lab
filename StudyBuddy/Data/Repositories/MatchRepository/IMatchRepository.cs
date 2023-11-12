using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories.MatchRepository;

public interface IMatchRepository
{
    Task<IEnumerable<Match>> GetAllAsync();
    Task AddAsync(UserId user1Id, UserId user2Id);
    Task<bool> IsMatchAsync(UserId currentUser, UserId otherUser);
    Task<List<Match>> GetMatchHistoryByUserIdAsync(UserId userId);
}
