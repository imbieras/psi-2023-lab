using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services.MatchingService;

public interface IMatchingService
{
    Task MatchUsersAsync(UserId currentUser, UserId otherUser);
    Task<bool> IsMatchedAsync(UserId currentUser, UserId otherUser);
    Task<IEnumerable<Match>> GetMatchHistoryAsync(UserId userId);
    Task<bool> IsRequestedMatchAsync(UserId currentUser, UserId otherUser);
}
