using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class UserSeenProfile
{
    public UserSeenProfile(UserId userId, UserId seenUserId)
    {
        UserId = userId;
        SeenUserId = seenUserId;
    }

    public UserSeenProfile(){}

    public UserId UserId { get; set; }
    public UserId SeenUserId { get; set; }
}
