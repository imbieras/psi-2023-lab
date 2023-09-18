using StudyBuddy.ValueObjects;

namespace StudyBuddy.Abstractions;

public interface IUserService
{
    UserId GetCurrentUserId();
    void SetCurrentUser(UserId userId);
}
