using FluentAssertions;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.Services;
using StudyBuddy.ValueObjects;

namespace StudyBuddyTests.Services;

public class GenericFilterServiceTests
{
    private readonly List<User> _users;

    public GenericFilterServiceTests()
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
            new UserTraits(DateTime.Today, "Education", User.GenerateGravatarUrl(user1Id), "Hello", Coordinates.From((0, 0))),
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
        _users = users;
    }

    [InlineData("Natural Sciences", 2)]
    [InlineData("Education", 1)]
    [InlineData("Reading", 0)]
    [Theory]
    public void FilterByPredicate_SpecifiedField_Returns_Users(string trait, int expectedCount)
    {
        List<User> actual = GenericFilterService<User>.FilterByPredicate(_users, user => user.Traits.Subject == trait);

        actual.Count.Should().Be(expectedCount);
    }

    [InlineData("Natural Sciences", "Playing an instrument", 2)]
    [InlineData("Natural Sciences", "Reading", 1)]
    [InlineData("Education", "Crafting", 0)]
    [Theory]
    public void FilterByPredicate_MultipleFields_Returns_Users(string trait, string hobby, int expectedCount)
    {
        List<User> actual = GenericFilterService<User>.FilterByPredicate(_users, user => user.Traits.Subject == trait && user.Hobbies!.Contains(hobby));

        actual.Count.Should().Be(expectedCount);
    }

}
