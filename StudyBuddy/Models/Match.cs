using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class Match
{
    public Match(UserId currentUser, UserId otherUser)
    {
        if (currentUser.CompareTo(otherUser) < 0)
        {
            User1Id = currentUser;
            User2Id = otherUser;
        }
        else
        {
            User1Id = otherUser;
            User2Id = currentUser;
        }

        MatchDate = DateTime.UtcNow;
        IsActive = true;
    }

    public int MatchId { get; set; }

    // Smaller UserId value is always User1Id
    public UserId User1Id { get; set; }
    public User User1 { get; set; } // Navigation property
    public UserId User2Id { get; set; }
    public User User2 { get; set; } // Navigation property

    public DateTime MatchDate { get; set; }
    public bool IsActive { get; set; }
}
