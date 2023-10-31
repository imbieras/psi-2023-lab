using StudyBuddy.Models;

namespace StudyBuddy.Controllers.ProfileController;

public class ProfileDto
{
    public string Name { get; set; }

    public string Birthdate { get; set; }

    public string Subject { get; set; }

    public IFormFile? Avatar { get; set; }

    public string? MarkdownContent { get; set; }

    public List<Hobby> Hobbies { get; set; } = new();

    public string? Longitude { get; set; } = null;

    public string? Latitude { get; set; } = null;
}
