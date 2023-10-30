using StudyBuddy.Abstractions;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class User : IUser, IEquatable<User>
{

    private User() { } // Private parameterless constructor for EF

    public User(UserId id, string name, UserFlags flags, UserTraits traits)
    {
        Id = id;
        Name = name;
        Flags = flags;
        Traits = traits;

        UsedIndexes = new List<int>();
    }

    public bool Equals(User? other)
    {
        if (other == null)
        {
            return false;
        }

        return Id == other.Id;
    }

    public List<int> UsedIndexes { get; set; }

    public UserId Id { get; }

    public string Name { get; set; }

    public UserFlags Flags { get; set; }

    public UserTraits Traits { get; set; }

    public ICollection<UserHobby> UserHobbies { get; set; } = new List<UserHobby>();

    public static string GenerateGravatarUrl(UserId userId) =>
        // Logic to create Gravatar URL based on user's unique identifier
        $"https://www.gravatar.com/avatar/{userId}?d=identicon";

    public override bool Equals(object? obj) => obj is User user && Equals(user);

    public override int GetHashCode() => Id.GetHashCode();
}
