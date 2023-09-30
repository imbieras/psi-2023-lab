using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Abstractions;

public interface IUser
{
    UserId Id { get; }

    string Name { get; }

    UserFlags Flags { get; }

    List<int> usedIndexes { get; set; }

    UserTraits Traits { get; set; }
}
