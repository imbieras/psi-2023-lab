using StudyBuddy.ValueObjects;

namespace StudyBuddy.Abstractions;

public interface IMatch
{
    public UserId User1Id { get; }

    public UserId User2Id { get; }

    public DateTime MatchDate { get; set; }
}
