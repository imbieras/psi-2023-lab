using System.ComponentModel.DataAnnotations;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Shared.Models;

public class UserTraits
{
    public UserTraits() {}

    public UserTraits(
        DateTime birthdate,
        string subject,
        string avatarPath,
        string description,
        Coordinates location
    )
    {
        Birthdate = birthdate;
        Subject = subject;
        AvatarPath = avatarPath;
        Description = description;
        Latitude = location.Value.Item1;
        Longitude = location.Value.Item2;
    }

    [Required]
    public DateTime Birthdate { get; set; } = DateTime.UtcNow;

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string AvatarPath { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;

    public double? Latitude { get; set; } = 0;

    public double? Longitude { get; set; } = 0;
}
