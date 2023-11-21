namespace StudyBuddy.Controllers.ProfileController;

public record ProfileDto
{
    public string Name { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string ConfirmPassword { get; init; } = string.Empty;// To check if they match

    public string Birthdate { get; init; } = string.Empty;

    public string Subject { get; init; } = string.Empty;

    public string MarkdownContent { get; init; } = string.Empty;

    public List<string> Hobbies { get; init; } = new();

    public string? Longitude { get; init; } = null;

    public string? Latitude { get; init; } = null;
}
