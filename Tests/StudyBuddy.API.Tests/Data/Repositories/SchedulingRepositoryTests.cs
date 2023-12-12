using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StudyBuddy.API.Data;
using StudyBuddy.API.Data.Repositories.SchedulingRepository;
using StudyBuddy.Shared.Models;

namespace StudyBuddyTests.Data.Repositories;

public class SchedulingRepositoryTests
{
    private StudyBuddyDbContext _dbContext;
    private readonly ILogger<SchedulingRepository> _logger;
    private readonly SchedulingRepository _sut;

    public SchedulingRepositoryTests()
    {
        DbContextOptions<StudyBuddyDbContext> options = new DbContextOptionsBuilder<StudyBuddyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new StudyBuddyDbContext(options);
        _logger = Substitute.For<ILogger<SchedulingRepository>>();

        List<Event> events = GenerateEvents();

        _sut = new SchedulingRepository(_dbContext, _logger);

        _dbContext.Schedules.AddRange(events);

        _dbContext.SaveChanges();
    }

    private static List<Event> GenerateEvents()
    {
        List<Event> events = new()
        {
            new Event
            {
                Id = 1,
                UserId = Guid.Parse("00000000-0000-0000-0000-111111111111"),
                Start = DateTime.UtcNow.AddHours(1),
                End = DateTime.UtcNow.AddHours(2),
                Text = "Event 1",
                Color = "Blue"
            },
            new Event
            {
                Id = 2,
                UserId = Guid.Parse("00000000-0000-0000-0000-111111111111"),
                Start = DateTime.UtcNow.AddHours(3),
                End = DateTime.UtcNow.AddHours(4),
                Text = "Event 2",
                Color = "Red"
            },
            new Event
            {
                Id = 3,
                UserId = Guid.Parse("00000000-0000-0000-0000-222222222222"),
                Start = DateTime.UtcNow.AddHours(5),
                End = DateTime.UtcNow.AddHours(6),
                Text = "Event 3",
                Color = "Green"
            },
            new Event
            {
                Id = 4,
                UserId = Guid.Parse("00000000-0000-0000-0000-333333333333"),
                Start = DateTime.UtcNow.AddHours(7),
                End = DateTime.UtcNow.AddHours(8),
                Text = "Event 4",
                Color = "Yellow"
            }
        };

        return events;
    }

    [Fact]
    public async Task GetEventsAsync_ExistingEventsInTimespan_Returns_AllEvents()
    {
        List<Event> expected = GenerateEvents();

        IEnumerable<Event> actual = await _sut.GetEventsAsync(DateTime.UtcNow, DateTime.UtcNow.AddHours(15));

        expected.Should().BeEquivalentTo(actual, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task GetEventsAsync_NoEventsInTimespan_Returns_EmptyList()
    {
        List<Event> expected = new();

        IEnumerable<Event> actual = await _sut.GetEventsAsync(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task GetEventsByUserIdAsync_ExistingEventsInTimespan_Returns_AllEvents()
    {
        List<Event> expected = GenerateEvents()
            .Where(e => e.UserId == Guid.Parse("00000000-0000-0000-0000-111111111111")).ToList();

        IEnumerable<Event> actual = await _sut.GetEventsByUserIdAsync(
            Guid.Parse("00000000-0000-0000-0000-111111111111"), DateTime.UtcNow, DateTime.UtcNow.AddHours(15));

        expected.Should().BeEquivalentTo(actual, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task GetEventsByUserIdAsync_NoEventsInTimespan_Returns_EmptyList()
    {
        List<Event> expected = new();

        IEnumerable<Event> actual = await _sut.GetEventsByUserIdAsync(
            Guid.Parse("00000000-0000-0000-0000-111111111111"), DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));

        expected.Should().BeEquivalentTo(actual);
    }

    [Fact]
    public async Task GetEventByIdAsync_ExistingId_Returns_Event()
    {
        List<Event> events = GenerateEvents();
        Event expected = events[0];

        Event? actual = await _sut.GetEventByIdAsync(expected.Id);

        expected.Should().BeEquivalentTo(actual, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task GetEventByIdAsync_NonExistingId_Returns_Null()
    {
        Event? @event = await _sut.GetEventByIdAsync(5);

        Assert.Null(@event);
    }

    [Fact]
    public async Task CreateEventAsync_NewEvent_Returns_Event()
    {
        Event expected = new()
        {
            Id = 5,
            UserId = Guid.Parse("00000000-0000-0000-0000-444444444444"),
            Start = DateTime.UtcNow.AddHours(9),
            End = DateTime.UtcNow.AddHours(10),
            Text = "Event 5",
            Color = "Purple"
        };

        Event actual = await _sut.CreateEventAsync(expected);

        expected.Should().BeEquivalentTo(actual, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task DeleteEventAsync_ExistingId_Deletes_Event()
    {
        List<Event> events = GenerateEvents();
        Event @event = events[0];

        await _sut.DeleteEventAsync(@event.Id);

        Event? actual = await _sut.GetEventByIdAsync(@event.Id);

        Assert.Null(actual);
    }

    [Fact]
    public async Task DeleteEventAsync_NonExistingId_Throws_ArgumentException()
    {
        Func<Task> action = async () => await _sut.DeleteEventAsync(5);

        await action.Should().ThrowAsync<ArgumentException>();
    }
}
