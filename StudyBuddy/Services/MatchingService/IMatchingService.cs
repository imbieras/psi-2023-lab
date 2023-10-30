namespace StudyBuddy.Services.MatchingService;

using StudyBuddy.Models;
using StudyBuddy.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IMatchingService
{
    Task MatchUsersAsync(UserId currentUser, UserId otherUser);
    Task<bool> IsMatchedAsync(UserId currentUser, UserId otherUser);
    Task<IEnumerable<Match>> GetMatchHistoryAsync(UserId userId);
}
