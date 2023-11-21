using StudyBuddy.Abstractions;
using StudyBuddy.Services.UserService;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services.UserSessionService;

public class UserSessionService : IUserSessionService
{
    private UserId? _currentUserId;
    public UserSessionService(IUserService userService) => _userService = userService;

    public UserId? GetCurrentUserId() => _currentUserId;

    public void SetCurrentUser(UserId userId) => _currentUserId = userId;

    private readonly IUserService _userService;

    public async Task<bool> AuthenticateUser(string username, string password)
    {
        IUser? user = await _userService.GetUserByUsernameAsync(username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return false;
        }

        SetCurrentUser(user.Id);
        return true;
    }
}
