using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudyBuddy.Data;
using StudyBuddy.Data.Repositories.MatchRepository;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddyTests.Data.Repositories;

public class MatchRepositoryTests
{
    private StudyBuddyDbContext _dbContext;
    private readonly ILogger<MatchRepository> _logger;
    private readonly MatchRepository _sut;

    public MatchRepositoryTests()
    {
        DbContextOptions<StudyBuddyDbContext> options = new DbContextOptionsBuilder<StudyBuddyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new StudyBuddyDbContext(options);
        _logger = new Logger<MatchRepository>(new LoggerFactory());

        List<Match> matches = GenerateMatches();

        _sut = new MatchRepository(_dbContext, _logger);

        _dbContext.Matches.AddRange(matches);

        _dbContext.SaveChanges();
    }

    private static List<Match> GenerateMatches()
    {
        UserId user1Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-111111111111"));
        UserId user2Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-222222222222"));
        UserId user3Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-333333333333"));

        List<Match> matches = new() { new Match(user1Id, user2Id), new Match(user1Id, user3Id), new Match(user2Id, user3Id) };

        return matches;
    }

    [Fact]
    public async Task GetAllAsync_Returns_AllMatches() => GenerateMatches().Should().BeEquivalentTo(await _sut.GetAllAsync(), options => options
        .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
        .WhenTypeIs<DateTime>());

    [Fact]
    public async Task AddAsync_Adds_Match()
    {
        UserId user1Id = UserId.From(Guid.Parse("88888888-8888-8888-8888-888888888888"));
        UserId user2Id = UserId.From(Guid.Parse("99999999-9999-9999-9999-999999999999"));

        Match match = new(user1Id, user2Id);

        List<Match> expected = GenerateMatches();
        expected.Add(match);

        await _sut.AddAsync(match.User1Id, match.User2Id);

        expected.Should().BeEquivalentTo(await _sut.GetAllAsync(), options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task RemoveAsync_ExistingMatch_Removes_Match()
    {
        List<Match> matches = GenerateMatches();
        Match match = matches[0];

        List<Match> expected = GenerateMatches();
        expected.RemoveAt(0);

        await _sut.RemoveAsync(match.User1Id, match.User2Id);

        IEnumerable<Match> actual = await _sut.GetAllAsync();

        expected.Should().BeEquivalentTo(actual, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task RemoveAsync_NonExistingMatch_Throws_ArgumentException()
    {
        UserId user1Id = UserId.From(Guid.Parse("88888888-8888-8888-8888-888888888888"));
        UserId user2Id = UserId.From(Guid.Parse("99999999-9999-9999-9999-999999999999"));

        Func<Task> action = async () => await _sut.RemoveAsync(user1Id, user2Id);

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task IsMatchAsync_ExistingMatch_Returns_True()
    {
        List<Match> matches = GenerateMatches();
        Match match = matches[0];

        const bool expected = true;

        bool actual = await _sut.IsMatchAsync(match.User1Id, match.User2Id);

        expected.Should().Be(actual);
    }

    [Fact]
    public async Task IsMatchAsync_NonExistingMatch_Returns_False()
    {
        UserId user1Id = UserId.From(Guid.Parse("88888888-8888-8888-8888-888888888888"));
        UserId user2Id = UserId.From(Guid.Parse("99999999-9999-9999-9999-999999999999"));

        const bool expected = false;

        bool actual = await _sut.IsMatchAsync(user1Id, user2Id);

        expected.Should().Be(actual);
    }

    [Fact]
    public async Task GetMatchHistoryByUserIdAsync_Returns_MatchHistory()
    {
        List<Match> matches = GenerateMatches();

        UserId user1Id = matches[0].User1Id;

        List<Match> expected = matches.Where(match => match.User1Id == user1Id || match.User2Id == user1Id).ToList();

        List<Match> actual = await _sut.GetMatchHistoryByUserIdAsync(user1Id);

        expected.Should().BeEquivalentTo(actual, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }
}
