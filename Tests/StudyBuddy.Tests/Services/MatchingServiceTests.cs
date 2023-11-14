using NSubstitute;
using StudyBuddy.Data.Repositories.MatchRepository;
using StudyBuddy.Models;
using StudyBuddy.Services.MatchingService;
using StudyBuddy.ValueObjects;

namespace StudyBuddyTests.Services;

public class MatchingServiceTests
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMatchRequestRepository _matchRequestRepository;
    private readonly MatchingService _sut;

    public MatchingServiceTests()
    {
        _matchRepository = Substitute.For<IMatchRepository>();
        _matchRequestRepository = Substitute.For<IMatchRequestRepository>();
        _sut = new MatchingService(_matchRepository, _matchRequestRepository);
    }

    [Fact]
    public async Task MatchUsersAsync_WhenMatchRequestExists_Adds_Match_And_Removes_MatchRequest()
    {
        var requesterId = UserId.From(Guid.NewGuid());
        var requestedId = UserId.From(Guid.NewGuid());
        _matchRequestRepository.IsMatchRequestExistsAsync(requestedId, requesterId).Returns(true);

        await _sut.MatchUsersAsync(requesterId, requestedId);

        await _matchRepository.Received(1).AddAsync(requesterId, requestedId);
        await _matchRequestRepository.Received(1).RemoveAsync(requestedId, requesterId);
    }

    [Fact]
    public async Task MatchUsersAsync_WhenMatchRequestDoesNotExist_Adds_MatchRequest()
    {
        var requesterId = UserId.From(Guid.NewGuid());
        var requestedId = UserId.From(Guid.NewGuid());
        _matchRequestRepository.IsMatchRequestExistsAsync(requestedId, requesterId).Returns(false);

        await _sut.MatchUsersAsync(requesterId, requestedId);

        await _matchRequestRepository.Received(1).AddAsync(requesterId, requestedId);
    }

    [InlineData(true)]
    [InlineData(false)]
    [Theory]
    public async Task IsRequestedMatchAsync_Returns_Expected(bool expected)
    {
        var requesterId = UserId.From(Guid.NewGuid());
        var requestedId = UserId.From(Guid.NewGuid());
        _matchRequestRepository.IsMatchRequestExistsAsync(requesterId, requestedId).Returns(expected);

        bool result = await _sut.IsRequestedMatchAsync(requesterId, requestedId);

        Assert.Equal(expected, result);
    }

    [InlineData(true)]
    [InlineData(false)]
    [Theory]
    public async Task IsMatchedAsync_Returns_Expected(bool expected)
    {
        var requesterId = UserId.From(Guid.NewGuid());
        var requestedId = UserId.From(Guid.NewGuid());
        _matchRepository.IsMatchAsync(requesterId, requestedId).Returns(expected);

        bool result = await _sut.IsMatchedAsync(requesterId, requestedId);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetMatchHistoryAsync_Returns_MatchHistory()
    {
        var userId = UserId.From(Guid.NewGuid());
        UserId user1Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-111111111111"));
        UserId user2Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-222222222222"));
        UserId user3Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-333333333333"));

        List<Match> expected = new() { new Match(user1Id, user2Id), new Match(user1Id, user3Id), new Match(user2Id, user3Id) };

        _matchRepository.GetMatchHistoryByUserIdAsync(userId).Returns(expected);

        var result = await _sut.GetMatchHistoryAsync(userId);

        Assert.Equal(expected, result);
    }
}
