using StudyBuddy.Abstractions;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class User : IUser, IEquatable<User>
{

    public User(UserId id, string name, UserFlags flags, UserTraits traits)
    {
        Id = id;
        Name = name;
        Flags = flags;
        Traits = traits;
    }

    public UserId Id { get; }

    public string Name { get; set; }

    public UserFlags Flags { get; set; }

    public UserTraits Traits { get; set; }

    public static string GenerateGravatarUrl(UserId userId) =>
        // Logic to create Gravatar URL based on user's unique identifier
        $"https://www.gravatar.com/avatar/{userId}?d=identicon";

    public bool Equals(User? other)
    {
        if (other == null) return false;

        return this.Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return obj is User user && Equals(user);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
