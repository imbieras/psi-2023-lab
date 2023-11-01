using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class Match
{
    public Match(UserId user1Id, UserId user2Id)
    {
        if (user1Id.CompareTo(user2Id) < 0)
        {
            User1Id = user1Id;
            User2Id = user2Id;
        }
        else
        {
            User1Id = user2Id;
            User2Id = user1Id;
        }

        MatchDate = DateTime.UtcNow;
        IsActive = true;
    }

    public Match()
    {
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
