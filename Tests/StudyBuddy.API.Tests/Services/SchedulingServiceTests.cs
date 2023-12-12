using NSubstitute;
using StudyBuddy.API.Data.Repositories.SchedulingRepository;
using StudyBuddy.API.Services.SchedulingService;
using StudyBuddy.Shared.Models;

namespace StudyBuddyTests.Services;

public class SchedulingServiceTests
{
    private readonly ISchedulingRepository _schedulingRepository;
    private readonly SchedulingService _sut;

    public SchedulingServiceTests()
    {
        _schedulingRepository = Substitute.For<ISchedulingRepository>();
        _sut = new SchedulingService(_schedulingRepository);
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
    public async Task GetEventsAsync_ExistingEventsInTimespan_Returns_AllEventsFromRepository()
    {
        List<Event> expected = GenerateEvents();
        DateTime start = DateTime.UtcNow.AddHours(-1);
        DateTime end = DateTime.UtcNow.AddHours(9);

        _schedulingRepository.GetEventsAsync(start, end).Returns(expected);

        List<Event> actual = await _sut.GetEventsAsync(start, end);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetEventsAsync_NoEventsInTimespan_Returns_EmptyList()
    {
        List<Event> expected = new();
        DateTime start = DateTime.UtcNow.AddHours(-1);
        DateTime end = DateTime.UtcNow.AddHours(9);

        _schedulingRepository.GetEventsAsync(start, end).Returns(expected);

        List<Event> actual = await _sut.GetEventsAsync(start, end);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetEventsByUserIdAsync_ExistingEventsInTimespan_Returns_AllEventsFromRepository()
    {
        List<Event> expected = GenerateEvents();
        DateTime start = DateTime.UtcNow.AddHours(-1);
        DateTime end = DateTime.UtcNow.AddHours(9);
        Guid userId = Guid.Parse("00000000-0000-0000-0000-111111111111");

        _schedulingRepository.GetEventsByUserIdAsync(userId, start, end).Returns(expected);

        List<Event> actual = await _sut.GetEventsByUserIdAsync(userId, start, end);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetEventsByUserIdAsync_NoEventsInTimespan_Returns_EmptyList()
    {
        List<Event> expected = new();
        DateTime start = DateTime.UtcNow.AddHours(-1);
        DateTime end = DateTime.UtcNow.AddHours(9);
        Guid userId = Guid.Parse("00000000-0000-0000-0000-111111111111");

        _schedulingRepository.GetEventsByUserIdAsync(userId, start, end).Returns(expected);

        List<Event> actual = await _sut.GetEventsByUserIdAsync(userId, start, end);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetEventByIdAsync_ExistingId_Returns_EventFromRepository()
    {
        List<Event> events = GenerateEvents();
        Event expected = events[0];

        _schedulingRepository.GetEventByIdAsync(expected.Id).Returns(expected);

        Event? actual = await _sut.GetEventByIdAsync(expected.Id);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetEventByIdAsync_NonExistingId_Returns_Null()
    {
        Event? expected = null;

        _schedulingRepository.GetEventByIdAsync(1).Returns(expected);

        Event? actual = await _sut.GetEventByIdAsync(1);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task CreateEventAsync_ValidEvent_Returns_EventFromRepository()
    {
        List<Event> events = GenerateEvents();
        Event expected = events[0];

        _schedulingRepository.CreateEventAsync(expected).Returns(expected);

        Event actual = await _sut.CreateEventAsync(expected);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UpdateEventAsync_ValidEvent_Returns_EventFromRepository()
    {
        List<Event> events = GenerateEvents();
        Event expected = events[0];

        _schedulingRepository.UpdateEventAsync(expected).Returns(expected);

        Event actual = await _sut.UpdateEventAsync(expected);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UpdateEventAsync_NonExistingEvent_Returns_Null()
    {
        Event? expected = null;

        _schedulingRepository.UpdateEventAsync(new Event())!.Returns(expected);

        Event? actual = await _sut.UpdateEventAsync(new Event());

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task DeleteEventAsync_ExistingId_Returns_EventFromRepository()
    {
        List<Event> events = GenerateEvents();
        Event expected = events[0];

        _schedulingRepository.DeleteEventAsync(expected.Id).Returns(expected);

        Event actual = await _sut.DeleteEventAsync(expected.Id);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task DeleteEventAsync_NonExistingId_Returns_Null()
    {
        Event? expected = null;

        _schedulingRepository.DeleteEventAsync(1)!.Returns(expected);

        Event? actual = await _sut.DeleteEventAsync(1);

        Assert.Equal(expected, actual);
    }
}
