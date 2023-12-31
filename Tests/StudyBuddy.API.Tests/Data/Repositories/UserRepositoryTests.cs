using Microsoft.EntityFrameworkCore;
using StudyBuddy.API.Data;
using StudyBuddy.API.Data.Repositories.UserRepository;
using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddyTests.Data.Repositories;

public class UserRepositoryTests
{
    private StudyBuddyDbContext _dbContext;
    private readonly UserRepository _sut;

    public UserRepositoryTests()
    {
        DbContextOptions<StudyBuddyDbContext> options = new DbContextOptionsBuilder<StudyBuddyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new StudyBuddyDbContext(options);

        List<User> users = GenerateUsers();

        _sut = new UserRepository(_dbContext);

        _dbContext.Users.AddRange(users);

        _dbContext.SaveChanges();
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
                new UserTraits(DateTime.Today, "Natural Sciences", User.GenerateGravatarUrl(user1Id), "Hello",
                    Coordinates.From((0, 0))),
                new List<string>()
            ),
            new User(
                user2Id,
                "Steve",
                "$2y$10$jEk3cJTygnRbTLeDr3bYQes32f.Y81sO4VRBDZUPLfuOPz9DDXoxu",
                UserFlags.Registered,
                new UserTraits(DateTime.Today.AddDays(-1), "Natural Sciences", User.GenerateGravatarUrl(user2Id),
                    "Hello", Coordinates.From((0, 0))),
                new List<string> { "Playing an instrument", "Painting" }
            ),
            new User(
                user3Id,
                "Peter",
                "$2y$10$olmb4AU5bN0fSt75mWTw9OwTDvrtpj5fNSI01zsYHSGjQEy1QVZT6",
                UserFlags.Registered,
                new UserTraits(DateTime.Today.AddDays(-2), "Natural Sciences", User.GenerateGravatarUrl(user3Id),
                    "Hello", Coordinates.From((0, 0))),
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
    public async Task GetAllAsync_Returns_AllUsers()
    {
        List<User> expected = GenerateUsers();

        IEnumerable<User> actual = await _sut.GetAllAsync();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_Returns_User()
    {
        List<User> users = GenerateUsers();
        User expected = users[0];

        User? actual = await _sut.GetByIdAsync(expected.Id);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_Returns_Null()
    {
        User? user = await _sut.GetByIdAsync(UserId.From(Guid.NewGuid()));

        Assert.Null(user);
    }

    [Fact]
    public async Task GetHobbiesByIdAsync_ExistingId_Returns_Hobbies()
    {
        List<User> users = GenerateUsers();
        User expected = users[1];

        List<string>? actual = await _sut.GetHobbiesByIdAsync(expected.Id);

        Assert.Equal(expected.Hobbies, actual);
    }

    [Fact]
    public async Task GetHobbiesByIdAsync_NonExistentId_Returns_Null()
    {
        List<string>? hobbies = await _sut.GetHobbiesByIdAsync(UserId.From(Guid.NewGuid()));

        Assert.Null(hobbies);
    }

    [Fact]
    public async Task UpdateAsync_ExistingUser_Updates_User()
    {
        List<User> users = GenerateUsers();
        User user = users[0];
        user.Username = "John Doe";
        user.Traits.Birthdate = DateTime.Today.AddDays(-5);

        await _sut.UpdateAsync(user);

        User? actual = await _sut.GetByIdAsync(user.Id);

        Assert.Equal(user, actual);
    }

    [Fact]
    public async Task UpdateAsync_ExistingUserWithHobbies_Updates_User()
    {
        List<User> users = GenerateUsers();
        User user = users[0];
        user.Username = "John Doe";
        user.Traits.Birthdate = DateTime.Today.AddDays(-5);
        user.Hobbies = new List<string> { "Natural Sciences", "Health Sciences" };

        await _sut.UpdateAsync(user);

        User? actual = await _sut.GetByIdAsync(user.Id);

        Assert.Equal(user, actual);
    }

    [Fact]
    public async Task DeleteAsync_ExistingUser_Deletes_User()
    {
        List<User> users = GenerateUsers();
        User user = users[0];

        await _sut.DeleteAsync(user.Id);

        User? actual = await _sut.GetByIdAsync(user.Id);

        Assert.Null(actual);
    }

    [Fact]
    public async Task UserExists_ExistingId_Returns_True()
    {
        List<User> users = GenerateUsers();
        User user = users[0];

        bool actual = await _sut.UserExistsAsync(user.Id);

        Assert.True(actual);
    }

    [Fact]
    public async Task UserExists_NonExistentId_Returns_False()
    {
        bool actual = await _sut.UserExistsAsync(UserId.From(Guid.NewGuid()));

        Assert.False(actual);
    }

    [Fact]
    public async Task UserSeenAsync_ExistingUser_Adds_UserSeenProfile()
    {
        List<User> users = GenerateUsers();
        User user = users[0];
        User otherUser = users[1];

        await _sut.UserSeenAsync(user.Id, otherUser.Id);

        bool actual = await _sut.IsUserSeenAsync(user.Id, otherUser.Id);

        Assert.True(actual);
    }

    [Fact]
    public async Task IsUserSeenAsync_ExistingUser_Returns_True()
    {
        List<User> users = GenerateUsers();
        User user = users[0];
        User otherUser = users[1];

        await _sut.UserSeenAsync(user.Id, otherUser.Id);

        bool actual = await _sut.IsUserSeenAsync(user.Id, otherUser.Id);

        Assert.True(actual);
    }

    [Fact]
    public async Task IsUserSeenAsync_NonExistentUser_Returns_False()
    {
        List<User> users = GenerateUsers();
        User user = users[0];
        User otherUser = users[1];

        bool actual = await _sut.IsUserSeenAsync(user.Id, otherUser.Id);

        Assert.False(actual);
    }

    [Fact]
    public async Task IsUserNotSeenAnyUserAsync_ExistingUser_Returns_True()
    {
        List<User> users = GenerateUsers();
        User user = users[0];

        bool actual = await _sut.IsUserNotSeenAnyUserAsync(user.Id);

        Assert.True(actual);
    }

    [Fact]
    public async Task IsUserNotSeenAnyUserAsync_ExistingUser_Returns_False()
    {
        List<User> users = GenerateUsers();
        User user = users[0];
        User otherUser = users[1];

        await _sut.UserSeenAsync(user.Id, otherUser.Id);

        bool actual = await _sut.IsUserNotSeenAnyUserAsync(user.Id);

        Assert.False(actual);
    }

    [Fact]
    public async Task GetUltimateSeenUserAsync_ExistingUser_Returns_UltimateSeenUser()
    {
        List<User> users = GenerateUsers();
        User user1 = users[0];
        User user2 = users[1];
        User user3 = users[2];

        await _sut.UserSeenAsync(user1.Id, user2.Id);
        await _sut.UserSeenAsync(user1.Id, user3.Id);

        UserId actual = await _sut.GetUltimateSeenUserAsync(user1.Id);

        Assert.Equal(user3.Id, actual);
    }

    [Fact]
    public async Task GetPenultimateSeenUserAsync_ExistingUser_Returns_PenultimateSeenUser()
    {
        List<User> users = GenerateUsers();
        User user1 = users[0];
        User user2 = users[1];
        User user3 = users[2];

        await _sut.UserSeenAsync(user1.Id, user2.Id);
        await _sut.UserSeenAsync(user1.Id, user3.Id);

        UserId actual = await _sut.GetPenultimateSeenUserAsync(user1.Id);

        Assert.Equal(user2.Id, actual);
    }

    [Fact]
    public async Task AddAsync_NonExistentUser_Adds_User()
    {
        User user = new(
            UserId.From(Guid.NewGuid()),
            "John",
            "$2y$10$MEGlkKcLY0nUK3XKZo05nuokVQJsmaxmC9wOlgpMgybAYzZgLUlvu",
            UserFlags.Registered,
            new UserTraits(DateTime.Today, "Natural Sciences", User.GenerateGravatarUrl(UserId.From(Guid.NewGuid())),
                "Hello", Coordinates.From((0, 0))),
            new List<string>()
        );

        await _sut.AddAsync(user);

        User? actual = await _sut.GetByIdAsync(user.Id);

        Assert.Equal(user, actual);
    }

    [Fact]
    public async Task AddAsync_ExistingUser_DoesNot_Add_User()
    {
        List<User> users = GenerateUsers();
        User user = users[0];

        await _sut.AddAsync(user);

        IEnumerable<User> actual = await _sut.GetAllAsync();

        Assert.Equal(users, actual);
    }
}
