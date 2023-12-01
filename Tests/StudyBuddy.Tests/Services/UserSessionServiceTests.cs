using NSubstitute;
using StudyBuddy.API.Services.UserService;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddyTests.Services;

public class UserSessionServiceTests
{
    private readonly UserSessionService _sut;

    public UserSessionServiceTests()
    {
        var httpClient = Substitute.For<HttpClient>();
        _sut = new UserSessionService(httpClient);
    }

    [Fact]
    public void GetCurrentUserId_NoUserSet_Returns_Null()
    {
        UserId? result = _sut.GetCurrentUserId();

        Assert.Null(result);
    }

    [Fact]
    public void SetCurrentUser_UserSet_Returns_User()
    {
        UserId userId = UserId.From(Guid.NewGuid());
        _sut.SetCurrentUser(userId);

        UserId? result = _sut.GetCurrentUserId();

        Assert.Equal(userId, result);
    }
}
