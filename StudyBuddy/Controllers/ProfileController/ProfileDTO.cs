namespace StudyBuddy.Controllers.ProfileController;

public class ProfileDto
{
    public string Name { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; } // To check if they match
    public string Birthdate { get; set; }
    public string Subject { get; set; }
    public IFormFile? Avatar { get; set; }
    public string? MarkdownContent { get; set; }
    public List<string> Hobbies { get; set; } = new();
    public string? Longitude { get; set; } = null;
    public string? Latitude { get; set; } = null;
}
