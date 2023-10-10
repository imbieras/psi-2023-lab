using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public struct UserTraits
{
    public DateTime Birthdate { get; set; }

    public string Subject { get; set; }

    public string AvatarPath { get; set; }

    public string Description { get; set; }

    public List<string> Hobbies { get; set; }

    public Coordinates Location { get; set; }

    public UserTraits(DateTime birthdate, string subject, string avatarPath, string description, List<string> hobbies,
        Coordinates location)
    {
        Birthdate = birthdate;
        Subject = subject;
        AvatarPath = avatarPath;
        Description = description;
        Hobbies = hobbies;
        Location = location;
    }
}
