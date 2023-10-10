using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Managers.MatchingManager;

public interface IMatchingManager
{
    void MatchUsers(UserId currentUser, UserId otherUser);
    bool IsRequestedMatch(UserId currentUser, UserId otherUser);
    bool IsMatched(UserId currentUser, UserId otherUser);
    List<Match> GetMatchHistory(UserId userId);
}
