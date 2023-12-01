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

    public DateTime Birthdate { get; set; } = DateTime.UtcNow;

    public string Subject { get; set; } = string.Empty;

    public string AvatarPath { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}
