using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Shared.DTOs;

public class MatchDto
{
    public UserId currentUserId { get; set; }
    public UserId otherUserId { get; set; }
}
