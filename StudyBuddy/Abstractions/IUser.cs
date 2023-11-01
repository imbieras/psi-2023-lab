using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Abstractions;

public interface IUser
{
    UserId Id { get; }

    string Name { get; }

    UserFlags Flags { get; }

    List<int> UsedIndexes { get; set; }

    UserTraits Traits { get; set; }
    public List<string>? Hobbies { get; set; }
}
