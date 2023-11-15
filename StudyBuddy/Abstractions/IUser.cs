using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Abstractions;

public interface IUser
{
    UserId Id { get; }

    string Name { get; }

    public string PasswordHash { get; set; }
    UserFlags Flags { get; }

    UserTraits Traits { get; set; }
    public List<string>? Hobbies { get; set; }
}
