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
    void MatchUsers(UserId currentUser, UserId otherUser);
    public bool IsMatched(UserId currentUser, UserId otherUser);
    List<IMatch> GetMatchHistory(UserId userId);
}
