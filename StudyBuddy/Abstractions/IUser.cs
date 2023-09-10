using StudyBuddy.ValueObjects;

namespace StudyBuddy.Abstractions;

public interface IUser
{
    UserId Id { get; }
    string Name { get; }
    UserFlags Flags { get; }
    DateTime Birthdate { get; }
    string Subject { get; }
    string AvatarFileName { get; set; }
}
