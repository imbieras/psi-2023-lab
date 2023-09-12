namespace StudyBuddy.Models;

public struct UserTraits
{
    public DateTime Birthdate { get; set; }

    public string Subject { get; set; }

    public string AvatarPath { get; set; }

    public UserTraits(DateTime birthdate, string subject, string avatarPath)
    {
        Birthdate = birthdate;
        Subject = subject;
        AvatarPath = avatarPath;
    }
}