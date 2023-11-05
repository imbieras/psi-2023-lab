using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class UserSeenProfile
{
    public UserSeenProfile(UserId userId, UserId seenUserId)
    {
        UserId = userId;
        SeenUserId = seenUserId;
        Timestamp = DateTime.UtcNow;

    }

    public UserSeenProfile()
    {
        Timestamp = DateTime.UtcNow;
    }

    public DateTime Timestamp { get; set; }
    public UserId UserId { get; set; }
    public UserId SeenUserId { get; set; }
}
