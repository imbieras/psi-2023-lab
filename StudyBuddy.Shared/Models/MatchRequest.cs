using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Shared.Models;

public class MatchRequest
{
    public MatchRequest()
    {
    }

    public MatchRequest(UserId currentUser, UserId otherUser)
    {
        RequesterId = currentUser;
        RequestedId = otherUser;
    }

    public UserId RequesterId { get; set; }

    public UserId RequestedId { get; set; }
}
