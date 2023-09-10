using StudyBuddy.ValueObjects;

namespace StudyBuddy.Abstractions;

public interface IUserManager
{
    UserId RegisterUser(string username, UserFlags flags, DateTime birthdate, string subject, string avatarFileName);

    IUser? GetUserById(UserId userId);

    IUser? GetUserByUsername(string username);

    void BanUser(UserId userId);
}
