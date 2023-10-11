using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services.UserService;

public interface IUserService
{
    UserId? GetCurrentUserId();
    void SetCurrentUser(UserId userId);
}
