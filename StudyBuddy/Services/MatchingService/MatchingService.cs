using StudyBuddy.Data.Repositories.MatchRepository;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services.MatchingService;

public class MatchingService : IMatchingService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchRequestRepository _matchRequestRepository;

    public MatchingService(IMatchRepository matchRepository, IMatchRequestRepository matchRequestRepository)
    {
        _matchRepository = matchRepository;
        _matchRequestRepository = matchRequestRepository;
    }

    public async Task MatchUsersAsync(UserId requesterId, UserId requestedId)
    {
        if (await IsRequestedMatchAsync(requestedId, requesterId))
        {
            await _matchRepository.AddAsync(requesterId, requestedId);
            await _matchRequestRepository.DeleteAsync(requestedId, requesterId);
        }
        else
        {
            await _matchRequestRepository.AddAsync(requesterId, requestedId);
        }
    }

    public async Task<bool> IsRequestedMatchAsync(UserId requesterId, UserId requestedId) =>
        await _matchRequestRepository.IsMatchRequestExistsAsync(requesterId, requestedId);

    public async Task<bool> IsMatchedAsync(UserId user1Id, UserId user2Id) =>
        await _matchRepository.IsMatchAsync(user1Id, user2Id);

    public async Task<IEnumerable<Match>> GetMatchHistoryAsync(UserId userId) =>
        await _matchRepository.GetMatchHistoryByUserIdAsync(userId);
}
