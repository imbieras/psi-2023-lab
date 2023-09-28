using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class Match
{
    public UserId User1Id { get; init; }

    public UserId User2Id { get; init; }

    public DateTime MatchDate { get; set; }
}
