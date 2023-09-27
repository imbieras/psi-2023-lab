using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;


namespace StudyBuddy.Managers;

public class UserManager : IUserManager
{
    private static readonly List<User?> s_users = new();
    private readonly Dictionary<UserId, List<UserId>?> _matches = new();
    private readonly List<IMatch> _matchHistory = new();
    private readonly Dictionary<UserId, List<UserId>?> _matchRequests = new();

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
        // Check if the other user has requested a match with the current user
        if (IsRequestedMatch(otherUser, currentUser))
        {
            // Add the match for both users
            AddMatch(currentUser, otherUser);
            AddMatch(otherUser, currentUser);

            // Remove the requests for both users
            RemoveMatchRequest(currentUser, otherUser);
            RemoveMatchRequest(otherUser, currentUser);

            // Add the match to the match history
            AddMatchToHistory(currentUser, otherUser);

            return;
        }

        // If there's no existing request, store the request from the current user
        AddMatchRequest(otherUser, currentUser);
    }

    public bool IsRequestedMatch(UserId currentUser, UserId otherUser) =>
        _matchRequests.ContainsKey(otherUser) && _matchRequests[otherUser]!.Contains(currentUser);

    public bool IsMatched(UserId currentUser, UserId otherUser)
    {
        if (_matches.TryGetValue(currentUser, out List<UserId>? currentUserMatches))
        {
            return currentUserMatches != null && currentUserMatches.Contains(otherUser);
        }

        return false;
    }

    public List<IMatch> GetMatchHistory(UserId userId) =>
        _matchHistory.Where(match => match.User1Id == userId || match.User2Id == userId).ToList();

    private void AddMatch(UserId userId, UserId matchUserId)
    {
        if (!_matches.ContainsKey(userId))
        {
            _matches[userId] = new List<UserId>();
        }

        if (!_matches[userId]!.Contains(matchUserId))
        {
            _matches[userId]!.Add(matchUserId);
        }
    }

    private void AddMatchRequest(UserId userId, UserId requestUserId)
    {
        if (!_matchRequests.ContainsKey(userId))
        {
            _matchRequests[userId] = new List<UserId>();
        }

        if (!_matchRequests[userId]!.Contains(requestUserId))
        {
            _matchRequests[userId]?.Add(requestUserId);
        }
    }

    private void RemoveMatchRequest(UserId userId, UserId requestUserId)
    {
        if (!_matchRequests.ContainsKey(userId))
        {
            return;
        }

        _matchRequests[userId]?.Remove(requestUserId);

        if (_matchRequests[userId]!.Count == 0)
        {
            _matchRequests.Remove(userId);
        }
    }

    private void AddMatchToHistory(UserId userId1, UserId userId2)
    {
        var match = new Match { User1Id = userId1, User2Id = userId2, MatchDate = DateTime.Now };
        _matchHistory.Add(match);
    }
}
