using System.ComponentModel.DataAnnotations.Schema;
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

    [ForeignKey ("UserId")]
    public UserId User1Id { get; set; }
    [ForeignKey ("UserId")]
    public UserId User2Id { get; set; }

    public DateTime MatchDate { get; set; }
    public bool IsActive { get; set; }
}
