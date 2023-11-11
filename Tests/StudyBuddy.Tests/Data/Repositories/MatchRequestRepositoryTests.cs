using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StudyBuddy.Data;
using StudyBuddy.Data.Repositories.MatchRepository;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddyTests.Data.Repositories;

public class MatchRequestRepositoryTests
{
    private StudyBuddyDbContext _dbContext;
    private readonly MatchRequestRepository _sut;

    public MatchRequestRepositoryTests()
    {
        DbContextOptions<StudyBuddyDbContext> options = new DbContextOptionsBuilder<StudyBuddyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new StudyBuddyDbContext(options);

        var matches = GenerateMatchRequests();

        _sut = new MatchRequestRepository(_dbContext);

        _dbContext.AddRange(matches);

        _dbContext.SaveChanges();
    }

    private static List<MatchRequest> GenerateMatchRequests()
    {
        var user1Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-111111111111"));
        var user2Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-222222222222"));
        var user3Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-333333333333"));

        var matches = new List<MatchRequest> { new(user1Id, user2Id), new(user1Id, user3Id), new(user2Id, user3Id) };

        return matches;
    }

    [Fact]
    public async Task GetAllAsync_Returns_AllMatchRequests()
    {
        GenerateMatchRequests().Should().BeEquivalentTo(await _sut.GetAllAsync());
    }

    [Fact]
    public async Task AddAsync_Adds_MatchRequest()
    {
        var user1Id = UserId.From(Guid.Parse("88888888-8888-8888-8888-888888888888"));
        var user2Id = UserId.From(Guid.Parse("99999999-9999-9999-9999-999999999999"));

        var matchRequest = new MatchRequest(user1Id, user2Id);

        var expected = GenerateMatchRequests();
        expected.Add(matchRequest);

        await _sut.AddAsync(user1Id, user2Id);

        expected.Should().BeEquivalentTo(await _sut.GetAllAsync());
    }

    [Fact]
    public async Task RemoveAsync_ExistingMatchRequest_Removes_MatchRequest()
    {
        var matchRequests = GenerateMatchRequests();
        var matchRequest = matchRequests[0];

        var expected = GenerateMatchRequests();
        expected.RemoveAt(0);

        await _sut.RemoveAsync(matchRequest.RequesterId, matchRequest.RequestedId);

        var actual = await _sut.GetAllAsync();

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task RemoveAsync_NonExistingMatchRequest_Throws_ArgumentException()
    {
        var user1Id = UserId.From(Guid.Parse("88888888-8888-8888-8888-888888888888"));
        var user2Id = UserId.From(Guid.Parse("99999999-9999-9999-9999-999999999999"));

        Func<Task> action = async () => await _sut.RemoveAsync(user1Id, user2Id);

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task IsMatchRequestExistsAsync_ExistingMatchRequest_Returns_True()
    {
        var matchRequests = GenerateMatchRequests();
        var matchRequest = matchRequests[0];

        const bool expected = true;

        bool actual = await _sut.IsMatchRequestExistsAsync(matchRequest.RequesterId, matchRequest.RequestedId);

        expected.Should().Be(actual);
    }

    [Fact]
    public async Task IsMatchRequestExistsAsync_NonExistingMatchRequest_Returns_False()
    {
        var user1Id = UserId.From(Guid.Parse("88888888-8888-8888-8888-888888888888"));
        var user2Id = UserId.From(Guid.Parse("99999999-9999-9999-9999-999999999999"));

        const bool expected = false;

        bool actual = await _sut.IsMatchRequestExistsAsync(user1Id, user2Id);

        expected.Should().Be(actual);
    }
}
