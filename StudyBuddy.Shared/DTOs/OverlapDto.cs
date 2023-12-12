namespace StudyBuddy.Shared.DTOs;

public class OverlapDto
{
    public Guid CurrentUserId { get; set; }
    public Guid OtherUserId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
}
