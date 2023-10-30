namespace StudyBuddy.Services.MatchingService;

using StudyBuddy.Data.Repositories;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;
using System.Threading.Tasks;

public class MatchingService : IMatchingService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchRequestRepository _matchRequestRepository;
    private readonly IUserRepository _userRepository;

    public MatchingService(IMatchRepository matchRepository, IMatchRequestRepository matchRequestRepository, IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _matchRequestRepository = matchRequestRepository;
        _userRepository = userRepository;
    }

    public async Task MatchUsersAsync(UserId currentUser, UserId otherUser)
    {
        if (await IsRequestedMatchAsync(otherUser, currentUser))
        {
            await _matchRepository.AddAsync(new Match(currentUser, otherUser));
            await _matchRequestRepository.DeleteAsync(currentUser, otherUser);
        }
        else
        {
            await _matchRequestRepository.AddAsync(new MatchRequest(currentUser, otherUser));
        }
    }

    private async Task<bool> IsRequestedMatchAsync(UserId currentUser, UserId otherUser) =>
        await _matchRequestRepository.IsMatchRequestExistsAsync(otherUser, currentUser);

    public async Task<bool> IsMatchedAsync(UserId currentUser, UserId otherUser) =>
        await _matchRepository.IsMatchAsync(currentUser, otherUser);

    public async Task<IEnumerable<Match>> GetMatchHistoryAsync(UserId userId) =>
        await _matchRepository.GetMatchHistoryByUserIdAsync(userId);
}
