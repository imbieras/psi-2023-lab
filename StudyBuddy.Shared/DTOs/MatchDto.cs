using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Shared.DTOs;

public class MatchDto
{
    public UserId CurrentUserId { get; set; }
    public UserId OtherUserId { get; set; }
}
