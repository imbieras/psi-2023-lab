namespace StudyBuddy.Controllers.ProfileController;

public class ProfileDTO
{
    public string Name { get; set; }
    public string Birthdate { get; set; }
    public string Subject { get; set; }
    public IFormFile? Avatar { get; set; }
    public string? MarkdownContent { get; set; }
    public List<string> Hobbies { get; set; }
    public string? Longitude { get; set; } = null;
    public string? Latitude { get; set; } = null;
}

