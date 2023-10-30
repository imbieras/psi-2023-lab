using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class MatchRequest
{
    public MatchRequest(UserId currentUser, UserId otherUser)
    {
        if (currentUser.CompareTo(otherUser) < 0)
        {
            RequesterId = currentUser;
            RequestedId = otherUser;
        }
        else
        {
            RequesterId = otherUser;
            RequestedId = currentUser;
        }
    }

    public int RequestId { get; set; }
    public UserId RequesterId { get; set; }
    public UserId RequestedId { get; set; }
}

