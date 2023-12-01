using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Services.UserSessionService;

public interface IUserSessionService
{
    UserId? GetCurrentUserId();
    void SetCurrentUser(UserId userId);
    Task<bool> AuthenticateUser(string username, string password);
}
