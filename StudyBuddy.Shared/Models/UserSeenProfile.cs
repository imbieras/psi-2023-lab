using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Shared.Models;

public class UserSeenProfile
{
    public UserSeenProfile() => Timestamp = DateTime.UtcNow;

    public UserSeenProfile(UserId userId, UserId seenUserId)
    {
        UserId = userId;
        SeenUserId = seenUserId;
        Timestamp = DateTime.UtcNow;
    }

    public DateTime Timestamp { get; set; }

    public UserId UserId { get; set; }

    public UserId SeenUserId { get; set; }
}
