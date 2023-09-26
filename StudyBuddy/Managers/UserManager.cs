using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;


namespace StudyBuddy.Managers;

public class UserManager : IUserManager
{
    private static readonly List<User?> s_users = new();
    private readonly Dictionary<UserId, UserId> _matches = new();
    private readonly List<IMatch> _matchHistory = new();

    public IUser? GetUserById(UserId userId) => s_users.FirstOrDefault(u => u?.Id == userId);

    public IUser? GetUserByUsername(string username) => s_users.FirstOrDefault(u => u?.Name == username);

    public void BanUser(UserId userId)
    {
        User? user = s_users.FirstOrDefault(u => u?.Id == userId);
        if (user != null)
        {
            user.Flags |= UserFlags.Banned;
        }
    }

    public UserId RegisterUser(string username, UserFlags flags, UserTraits traits)
    {
        UserId userId = UserId.From(Guid.NewGuid());

        if (string.IsNullOrEmpty(traits.AvatarPath))
        {
            traits.AvatarPath = User.GenerateGravatarUrl(userId);
        }

        User newUser = new(userId, username, flags, traits);
        s_users.Add(newUser);

        return userId;
    }

    public List<IUser> GetAllUsers() => s_users.Where(u => u != null).Cast<IUser>().ToList();

    public void MatchUsers(UserId currentUser, UserId otherUser)
    {
        if (_matches.ContainsKey(currentUser) || _matches.ContainsKey(otherUser))
        {
            return;
        }

        _matches.Add(currentUser, otherUser);

        _matchHistory.Add(new Match { User1Id = currentUser, User2Id = otherUser, MatchDate = DateTime.Now });
    }

    public bool IsMatched(UserId currentUser, UserId otherUser) => _matches.ContainsKey(currentUser) && _matches[currentUser] == otherUser;

    public List<IMatch> GetMatchHistory(UserId userId) => _matchHistory.Where(match => match.User1Id == userId || match.User2Id == userId).ToList();
}
