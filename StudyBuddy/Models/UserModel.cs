using StudyBuddy.Abstractions;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class User : IUser
{
    public UserId Id { get; private set; }
    public string Name { get; set; }
    public UserFlags Flags { get; set; }
    public DateTime Birthdate { get; set; }
    public string Subject { get; set; }
    public string AvatarFileName { get; set; }

    public User(UserId id, string name, UserFlags flags, DateTime birthdate, string subject, string avatarFileName)
    {
        Id = id;
        Name = name;
        Flags = flags;
        Birthdate = birthdate;
        Subject = subject;
        AvatarFileName = avatarFileName;
    }
}
