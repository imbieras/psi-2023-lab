using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services.UserService;

public class UserService : IUserService
{
    private UserId? _currentUserId;

    public UserId? GetCurrentUserId() => _currentUserId;

    public void SetCurrentUser(UserId userId) => _currentUserId = userId;
}
