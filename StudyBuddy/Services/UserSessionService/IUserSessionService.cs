using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services.UserSessionService;

public interface IUserSessionService
{
    UserId? GetCurrentUserId();
    void SetCurrentUser(UserId userId);
}
