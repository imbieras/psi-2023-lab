using System.ComponentModel.DataAnnotations;

namespace StudyBuddy.Shared.DTOs;

public class UpdateUserDto
{
    [Required] public string Subject { get; set; } = string.Empty;

    public string? MarkdownContent { get; set; } = string.Empty;
    public List<string>? Hobbies { get; set; } = new();
}
