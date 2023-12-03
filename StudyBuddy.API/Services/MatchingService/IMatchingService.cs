using StudyBuddy.Shared.DTOs;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Services.MatchingService;

public interface IMatchingService
{
    Task MatchUsersAsync(MatchDto matchDto);
    Task<bool> IsMatchedAsync(UserId currentUser, UserId otherUser);
    Task<IEnumerable<Match>> GetMatchHistoryAsync(UserId userId);
    Task<bool> IsRequestedMatchAsync(UserId currentUser, UserId otherUser);
}
