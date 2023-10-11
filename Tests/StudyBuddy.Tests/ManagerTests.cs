using NSubstitute;
using StudyBuddy.Abstractions;
using StudyBuddy.Managers.FileManager;
using StudyBuddy.Managers.MatchingManager;
using StudyBuddy.Managers.UserManager;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddyTests;

public class ManagerTests
{
    private readonly UserManager _userManager;
    private readonly MatchingManager _matchingManager;
    private readonly FileManager _fileManager;

    public ManagerTests()
    {
        _userManager = new UserManager();
        _matchingManager = new MatchingManager();
        _userManager = Substitute.For<UserManager>();
        _fileManager = new FileManager(_userManager);
    }

    [Fact]
    public void RegisterUser_ValidUser_ReturnsUserId()
    {
        // Arrange
        string username = "TestUser";
        var flags = UserFlags.Inactive;
        var traits = new UserTraits();

        // Act
        var userId = _userManager.RegisterUser(username, flags, traits);
        IUser? registeredUser = _userManager.GetUserById(userId);

        // Assert
        Assert.NotNull(registeredUser);
        Assert.Equal(username, registeredUser.Name);
        Assert.Equal(flags, registeredUser.Flags);
    }
/*
    [Fact]
    public void GetPreviousRandomProfile_ValidUser_ReturnsPreviousProfile()
    {
        // Arrange
        var user = Substitute.For<IUser>();
       // var previousProfile = Substitute.For<IUser>();
        user.UsedIndexes.Returns(new List<int>()); // Ensure that UsedIndexes is empty.
        _userManager.RegisterUser("User1", UserFlags.Inactive, new UserTraits());
        _userManager.RegisterUser("User2", UserFlags.Inactive, new UserTraits());

        // Act
        var result = _userManager.GetPreviousRandomProfile(user);
        var users = _userManager.GetAllUsers();

        // Assert
        Assert.Null(result); // Expecting null when UsedIndexes is empty.
        Assert.NotEmpty(users); // Ensure the list is not empty.
        Assert.Equal(2, users.Count);
    }
    */

    [Fact]
    public void GetPreviousRandomProfile_IndexOutOfRange_ReturnsNull()
    {
        // Arrange
        IUser user = Substitute.For<IUser>();
        user.UsedIndexes.Returns(new List<int> { 5 }); // One used index, but it's out of range.

        // Act
        IUser? previousProfile = _userManager.GetPreviousRandomProfile(user);

        // Assert
        Assert.Null(previousProfile);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void GetRandomUser_ValidUser_ReturnsDifferentUser(int usedIndexesCount)
    {
        // Arrange
        IUser user = Substitute.For<IUser>();
        user.UsedIndexes.Returns(new List<int>());
        var users = new List<IUser>
        {
            Substitute.For<IUser>(), // User 0
            Substitute.For<IUser>(), // User 1
            Substitute.For<IUser>(), // User 2
        };

        for (int i = 0; i < usedIndexesCount; i++)
        {
            user.UsedIndexes.Add(i);
        }
        UserManager userManager = new UserManager();

        // Act
        IUser? randomUser = userManager.GetRandomUser(user);

        // Assert
        if ((users.Count) != usedIndexesCount)
        {
            Assert.Null(randomUser);
        }
        else
        {
            Assert.NotNull(randomUser);
            Assert.NotEqual(user, randomUser);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void MatchUsers_ValidMatchRequest_MatchesUsers(int numberOfMatches)
    {
        // Arrange
        var currentUser = UserId.From(Guid.NewGuid());
        var otherUser = UserId.From(Guid.NewGuid());

        // Create match requests
        for (int i = 0; i < numberOfMatches; i++)
        {
            _matchingManager.AddMatchRequest(currentUser, otherUser);
            _matchingManager.MatchUsers(currentUser, otherUser);
        }

        // Act
        bool isMatched = _matchingManager.IsMatched(currentUser, otherUser);

        // Assert
        if (numberOfMatches > 0)
        {
            Assert.True(isMatched);
        }
        else
        {
            Assert.False(isMatched);
        }
    }

    [Fact]
    public void LoadUsersFromCsv_FileDoesNotExist_DoesNotLoadUsers()
    {
        // Arrange
        string filePath = "nonexistent.csv";

        // Act
        _fileManager.LoadUsersFromCsv(filePath);

        // Assert
        _userManager.DidNotReceive().RegisterUser(Arg.Any<string>(), Arg.Any<UserFlags>(), Arg.Any<UserTraits>());
    }

}