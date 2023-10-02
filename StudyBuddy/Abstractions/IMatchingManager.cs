using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Abstractions;

public interface IMatchingManager
{
    void MatchUsers(UserId currentUser, UserId otherUser);
    bool IsRequestedMatch(UserId currentUser, UserId otherUser);
    bool IsMatched(UserId currentUser, UserId otherUser);
    List<Match> GetMatchHistory(UserId userId);
}
