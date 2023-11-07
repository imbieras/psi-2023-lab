using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class MatchRequest
{
    public MatchRequest(UserId currentUser, UserId otherUser)
    {
        RequesterId = currentUser;
        RequestedId = otherUser;
    }

    public MatchRequest()
    {
    }

    public UserId RequesterId { get; set; }
    public UserId RequestedId { get; set; }
}
