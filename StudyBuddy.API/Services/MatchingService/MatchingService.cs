using StudyBuddy.API.Data.Repositories.MatchRepository;
using StudyBuddy.Shared.DTOs;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Services.MatchingService;

public class MatchingService : IMatchingService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchRequestRepository _matchRequestRepository;

    public MatchingService(IMatchRepository matchRepository, IMatchRequestRepository matchRequestRepository)
    {
        _matchRepository = matchRepository;
        _matchRequestRepository = matchRequestRepository;
    }

    public async Task MatchUsersAsync(MatchDto matchDto)
    {
        if (await IsRequestedMatchAsync(matchDto.otherUserId, matchDto.currentUserId))
        {
            await _matchRepository.AddAsync(matchDto.currentUserId, matchDto.otherUserId);
            await _matchRequestRepository.RemoveAsync(matchDto.otherUserId, matchDto.currentUserId);
        }
        else
        {
            await _matchRequestRepository.AddAsync(matchDto.currentUserId, matchDto.otherUserId);
        }
    }

    public async Task<bool> IsRequestedMatchAsync(UserId requesterId, UserId requestedId) =>
        await _matchRequestRepository.IsMatchRequestExistsAsync(requesterId, requestedId);

    public async Task<bool> IsMatchedAsync(UserId user1Id, UserId user2Id) =>
        await _matchRepository.IsMatchAsync(user1Id, user2Id);

    public async Task<IEnumerable<Match>> GetMatchHistoryAsync(UserId userId) =>
        await _matchRepository.GetMatchHistoryByUserIdAsync(userId);
}
