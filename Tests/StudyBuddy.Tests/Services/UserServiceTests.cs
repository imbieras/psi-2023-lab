using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StudyBuddy.API.Data.Repositories.UserRepository;
using StudyBuddy.API.Services.UserService;
using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.DTOs;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddyTests.Services;

public class UserServiceTests
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _logger = Substitute.For<ILogger<UserService>>();

        _sut = new UserService(_userRepository, _logger);
    }

    private static List<User> GenerateUsers()
    {
        UserId user1Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-111111111111"));
        UserId user2Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-222222222222"));
        UserId user3Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-333333333333"));

        List<User> users = new()
        {
            new User(
            user1Id,
            "John",
            "$2y$10$MEGlkKcLY0nUK3XKZo05nuokVQJsmaxmC9wOlgpMgybAYzZgLUlvu",
            UserFlags.Registered,
            new UserTraits(DateTime.Today, "Natural Sciences", User.GenerateGravatarUrl(user1Id), "Hello", Coordinates.From((0, 0))),
            new List<string>()
            ),
            new User(
            user2Id,
            "Steve",
            "$2y$10$jEk3cJTygnRbTLeDr3bYQes32f.Y81sO4VRBDZUPLfuOPz9DDXoxu",
            UserFlags.Registered,
            new UserTraits(DateTime.Today.AddDays(-1), "Natural Sciences", User.GenerateGravatarUrl(user2Id), "Hello", Coordinates.From((0, 0))),
            new List<string> { "Playing an instrument", "Painting" }
            ),
            new User(
            user3Id,
            "Peter",
            "$2y$10$olmb4AU5bN0fSt75mWTw9OwTDvrtpj5fNSI01zsYHSGjQEy1QVZT6",
            UserFlags.Registered,
            new UserTraits(DateTime.Today.AddDays(-2), "Natural Sciences", User.GenerateGravatarUrl(user3Id), "Hello", Coordinates.From((0, 0))),
            new List<string>
            {
                "Reading",
                "Drawing",
                "Playing an instrument",
                "Painting",
                "Cooking",
                "Gardening",
                "Photography",
                "Writing",
                "Hiking",
                "Cycling",
                "Singing",
                "Dancing",
                "Yoga",
                "Meditation",
                "Chess",
                "Video gaming",
                "Binge-watching",
                "Crafting",
                "Collecting",
                "Baking",
                "Fitness",
                "Rock climbing",
                "Skiing or snowboarding",
                "Surfing",
                "Scuba diving",
                "Tabletop gaming",
                "Birdwatching",
                "Volunteering",
                "Traveling",
                "Auto mechanics"
            }
            )
        };
        return users;
    }

    [Fact]
    public async Task GetAllUsersAsync_Returns_AllUsersFromRepository()
    {
        List<User> expected = GenerateUsers();
        _userRepository.GetAllAsync().Returns(expected);

        var actual = await _sut.GetAllUsersAsync();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingUser_Returns_UserFromRepository()
    {
        List<User> expected = GenerateUsers();
        _userRepository.GetByIdAsync(expected[0].Id).Returns(expected[0]);

        var actual = await _sut.GetUserByIdAsync(expected[0].Id);

        Assert.Equal(expected[0], actual);
    }

    [Fact]
    public async Task GetUserByIdAsync_NonExistingUser_Returns_Null()
    {
        List<User> expected = GenerateUsers();
        _userRepository.GetByIdAsync(expected[0].Id).Returns((User?)null);

        var actual = await _sut.GetUserByIdAsync(expected[0].Id);

        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetHobbiesByIdAsync_ExistingUser_Returns_HobbiesFromRepository()
    {
        List<User> expected = GenerateUsers();
        _userRepository.GetHobbiesByIdAsync(expected[0].Id).Returns(expected[0].Hobbies);

        var actual = await _sut.GetHobbiesById(expected[0].Id);

        Assert.Equal(expected[0].Hobbies, actual);
    }

    [Fact]
    public async Task GetHobbiesByIdAsync_NonExistingUser_Returns_Null()
    {
        List<User> expected = GenerateUsers();
        _userRepository.GetHobbiesByIdAsync(expected[0].Id).Returns((List<string>?)null);

        var actual = await _sut.GetHobbiesById(expected[0].Id);

        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetUserByUsernameAsync_ExistingUser_Returns_UserFromRepository()
    {
        List<User> expected = GenerateUsers();
        _userRepository.GetAllAsync().Returns(expected);

        var actual = await _sut.GetUserByUsernameAsync(expected[0].Username);

        Assert.Equal(expected[0], actual);
    }

    [Fact]
    public async Task GetUserByUsernameAsync_NonExistingUser_Returns_Null()
    {
        List<User> expected = GenerateUsers();
        _userRepository.GetAllAsync().Returns(expected);

        var actual = await _sut.GetUserByUsernameAsync("NonExistingUser");

        actual.Should().BeNull();
    }

    [Fact]
    public async Task BanUserAsync_ExistingUser_Bans_User()
    {
        List<User> expected = GenerateUsers();
        _userRepository.GetByIdAsync(expected[0].Id).Returns(expected[0]);

        await _sut.BanUserAsync(expected[0].Id);

        await _userRepository.Received(1).UpdateAsync(expected[0]);
    }

    [Fact]
    public async Task BanUserAsync_NonExistingUser_DoesNotBan_User()
    {
        List<User> expected = GenerateUsers();
        _userRepository.GetByIdAsync(expected[0].Id).Returns((User?)null);

        await _sut.BanUserAsync(expected[0].Id);

        await _userRepository.DidNotReceive().UpdateAsync(expected[0]);
    }

    [Fact]
    public async Task UpdateAsync_ExistingUser_Updates_User()
    {
        List<User> users = GenerateUsers();
        User user = users[0];

        UpdateUserDto updateUserDto = new()
        {
            Subject = "Social Sciences",
            Hobbies = new List<string> { "Playing an instrument", "Painting" },
            MarkdownContent = "Hello"
        };

        _userRepository.GetByIdAsync(user.Id).Returns(user);

        await _sut.UpdateAsync(user.Id, updateUserDto);

        await _userRepository.Received(1).UpdateAsync(user.Id, updateUserDto);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingUser_DoesNotUpdate_User()
    {
        List<User> users = GenerateUsers();
        User user = users[0];

        UpdateUserDto updateUserDto = new()
        {
            Subject = "Social Sciences",
            Hobbies = new List<string> { "Playing an instrument", "Painting" },
            MarkdownContent = "Hello"
        };


        _userRepository.GetByIdAsync(user.Id).Returns((User?)null);

        await _userRepository.DidNotReceive().UpdateAsync(user.Id, updateUserDto);
    }

    [Fact]
    public async Task GetRandomUserAsync_ExistingUsers_Returns_RandomUser()
    {
        List<User> users = GenerateUsers();
        _userRepository.GetAllAsync().Returns(users);

        var actual = await _sut.GetRandomUserAsync();

        Assert.Contains(actual, users);
    }

    [Fact]
    public async Task GetRandomUserAsync_NonExistingUsers_Returns_Null()
    {
        List<User> users = new();
        _userRepository.GetAllAsync().Returns(users);

        var actual = await _sut.GetRandomUserAsync();

        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetUltimateSeenUserAsync_ReturnsUserFromRepository()
    {
        UserId userId = UserId.From(Guid.NewGuid());
        UserId latestSeenUserId = UserId.From(Guid.NewGuid());
        UserId user1Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-111111111111"));
        User expected = new(
        user1Id,
        "John",
        "$2y$10$MEGlkKcLY0nUK3XKZo05nuokVQJsmaxmC9wOlgpMgybAYzZgLUlvu",
        UserFlags.Registered,
        new UserTraits(DateTime.Today, "Natural Sciences", User.GenerateGravatarUrl(user1Id), "Hello", Coordinates.From((0, 0))),
        new List<string>()
        );

        _userRepository.GetUltimateSeenUserAsync(userId).Returns(latestSeenUserId);
        _userRepository.GetByIdAsync(latestSeenUserId).Returns(expected);

        var actual = await _sut.GetUltimateSeenUserAsync(userId);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetUltimateSeenUserAsync_NonExistingUser_Returns_Null()
    {
        List<User> users = GenerateUsers();
        _userRepository.GetUltimateSeenUserAsync(users[0].Id).Returns(UserId.From(Guid.NewGuid()));

        var actual = await _sut.GetUltimateSeenUserAsync(users[0].Id);

        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetPenultimateSeenUserAsync_ReturnsUserFromRepository()
    {
        UserId userId = UserId.From(Guid.NewGuid());
        UserId penultimateSeenUserId = UserId.From(Guid.NewGuid());
        UserId user1Id = UserId.From(Guid.Parse("00000000-0000-0000-0000-111111111111"));
        User expected = new(
        user1Id,
        "John",
        "$2y$10$MEGlkKcLY0nUK3XKZo05nuokVQJsmaxmC9wOlgpMgybAYzZgLUlvu",
        UserFlags.Registered,
        new UserTraits(DateTime.Today, "Natural Sciences", User.GenerateGravatarUrl(user1Id), "Hello", Coordinates.From((0, 0))),
        new List<string>()
        );

        _userRepository.GetPenultimateSeenUserAsync(userId).Returns(penultimateSeenUserId);
        _userRepository.GetByIdAsync(penultimateSeenUserId).Returns(expected);

        var actual = await _sut.GetPenultimateSeenUserAsync(userId);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetPenultimateSeenUserAsync_NonExistingUser_Returns_Null()
    {
        List<User> users = GenerateUsers();
        _userRepository.GetPenultimateSeenUserAsync(users[0].Id).Returns(UserId.From(Guid.NewGuid()));

        var actual = await _sut.GetPenultimateSeenUserAsync(users[0].Id);

        actual.Should().BeNull();
    }

    [InlineData(true)]
    [InlineData(false)]
    [Theory]
    public async Task IsUserNotSeenAnyUserAsync_Returns_Expected(bool expected)
    {
        List<User> users = GenerateUsers();
        User user = users[0];

        _userRepository.IsUserNotSeenAnyUserAsync(user.Id).Returns(expected);

        bool actual = await _sut.IsUserNotSeenAnyUserAsync(user.Id);

        Assert.Equal(expected, actual);
    }

    [InlineData(true)]
    [InlineData(false)]
    [Theory]
    public async Task IsUserSeenAsync_Returns_Expected(bool expected)
    {
        List<User> users = GenerateUsers();
        User user = users[0];
        User otherUser = users[1];

        _userRepository.IsUserSeenAsync(user.Id, otherUser.Id).Returns(expected);

        bool actual = await _sut.IsUserSeenAsync(user.Id, otherUser.Id);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task UserSeenAsync_User_Sees_User()
    {
        List<User> users = GenerateUsers();
        User user = users[0];
        User otherUser = users[1];

        await _sut.UserSeenAsync(user.Id, otherUser.Id);

        await _userRepository.Received(1).UserSeenAsync(user.Id, otherUser.Id);
    }
}
