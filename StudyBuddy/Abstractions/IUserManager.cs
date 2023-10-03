using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Abstractions;

public interface IUserManager
{
    UserId RegisterUser(string username, UserFlags flags, UserTraits traits);
    IUser? GetUserById(UserId userId);
    IUser? GetUserByUsername(string username);
    void BanUser(UserId userId);
    List<IUser> GetAllUsers();
    IUser? GetRandomUser(IUser user);
    IUser? GetPreviousRandomProfile(IUser user);
    IUser? GetCurrentRandomUser(IUser user);
    bool IsUsedIndexesEmpty(IUser user);
}
