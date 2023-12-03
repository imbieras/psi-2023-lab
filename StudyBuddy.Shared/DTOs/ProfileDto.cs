using System.ComponentModel.DataAnnotations;

namespace StudyBuddy.Shared.DTOs;

public record ProfileDto
{
    [Required] public string Username { get; init; } = string.Empty;

    [Required] public string Password { get; init; } = string.Empty;

    [Required] public string ConfirmPassword { get; init; } = string.Empty; // To check if they match

    [Required] public string Birthdate { get; init; } = string.Empty;

    [Required] public string Subject { get; init; } = string.Empty;

    public string? MarkdownContent { get; init; } = string.Empty;

    public List<string>? Hobbies { get; init; } = new();

    public string? Longitude { get; init; } = string.Empty;

    public string? Latitude { get; init; } = string.Empty;
}
