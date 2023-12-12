using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyBuddy.API.Data;
using StudyBuddy.API.Data.Repositories.MatchRepository;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddyTests.Data.Repositories;

public class MatchRequestRepositoryTests
{
    private StudyBuddyDbContext _dbContext;
    private readonly ILogger<MatchRequestRepository> _logger;
    private readonly MatchRequestRepository _sut;

    public MatchRequestRepositoryTests()
    {
        DbContextOptions<StudyBuddyDbContext> options = new DbContextOptionsBuilder<StudyBuddyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new StudyBuddyDbContext(options);
        _logger = new Logger<MatchRequestRepository>(new LoggerFactory());

        List<MatchRequest> matchRequests = GenerateMatchRequests();

        _sut = new MatchRequestRepository(_dbContext, _logger);

        _dbContext.MatchRequests.AddRange(matchRequests);

        _dbContext.SaveChanges();
    }

    private static List<MatchRequest> GenerateMatchRequests()
    {
        UserId user1Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-111111111111"));
        UserId user2Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-222222222222"));
        UserId user3Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-333333333333"));

        List<MatchRequest> matches = new()
        {
            new MatchRequest(user1Id, user2Id),
            new MatchRequest(user1Id, user3Id),
            new MatchRequest(user2Id, user3Id)
        };

        return matches;
    }

    [Fact]
    public async Task GetAllAsync_Returns_AllMatchRequests() =>
        GenerateMatchRequests().Should().BeEquivalentTo(await _sut.GetAllAsync());

    [Fact]
    public async Task AddAsync_Adds_MatchRequest()
    {
        UserId user1Id = UserId.From(Guid.Parse("88888888-8888-8888-8888-888888888888"));
        UserId user2Id = UserId.From(Guid.Parse("99999999-9999-9999-9999-999999999999"));

        MatchRequest matchRequest = new(user1Id, user2Id);

        List<MatchRequest> expected = GenerateMatchRequests();
        expected.Add(matchRequest);

        await _sut.AddAsync(user1Id, user2Id);

        expected.Should().BeEquivalentTo(await _sut.GetAllAsync());
    }

    [Fact]
    public async Task RemoveAsync_ExistingMatchRequest_Removes_MatchRequest()
    {
        List<MatchRequest> matchRequests = GenerateMatchRequests();
        MatchRequest matchRequest = matchRequests[0];

        List<MatchRequest> expected = GenerateMatchRequests();
        expected.RemoveAt(0);

        await _sut.RemoveAsync(matchRequest.RequesterId, matchRequest.RequestedId);

        IEnumerable<MatchRequest> actual = await _sut.GetAllAsync();

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task RemoveAsync_NonExistingMatchRequest_Throws_ArgumentException()
    {
        UserId user1Id = UserId.From(Guid.Parse("88888888-8888-8888-8888-888888888888"));
        UserId user2Id = UserId.From(Guid.Parse("99999999-9999-9999-9999-999999999999"));

        Func<Task> action = async () => await _sut.RemoveAsync(user1Id, user2Id);

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task IsMatchRequestExistsAsync_ExistingMatchRequest_Returns_True()
    {
        List<MatchRequest> matchRequests = GenerateMatchRequests();
        MatchRequest matchRequest = matchRequests[0];

        const bool expected = true;

        bool actual = await _sut.IsMatchRequestExistsAsync(matchRequest.RequesterId, matchRequest.RequestedId);

        expected.Should().Be(actual);
    }

    [Fact]
    public async Task IsMatchRequestExistsAsync_NonExistingMatchRequest_Returns_False()
    {
        UserId user1Id = UserId.From(Guid.Parse("88888888-8888-8888-8888-888888888888"));
        UserId user2Id = UserId.From(Guid.Parse("99999999-9999-9999-9999-999999999999"));

        const bool expected = false;

        bool actual = await _sut.IsMatchRequestExistsAsync(user1Id, user2Id);

        expected.Should().Be(actual);
    }
}
