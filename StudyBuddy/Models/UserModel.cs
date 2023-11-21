using StudyBuddy.Abstractions;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class User : IUser, IEquatable<User>
{
    private User()
    {
    } // Private parameterless constructor for EF

    public User(UserId id, string name, string passwordHash, UserFlags flags, UserTraits traits, List<string>? hobbies)
    {
        Id = id;
        Name = name;
        PasswordHash = passwordHash;
        Flags = flags;
        Traits = traits;
        Hobbies = hobbies;
    }

    public bool Equals(User? other)
    {
        if (other == null)
        {
            return false;
        }

        return Id == other.Id;
    }


    public UserId Id { get; }

    public string Name { get; set; }

    public string PasswordHash { get; set; }
    public UserFlags Flags { get; set; }

    public UserTraits Traits { get; set; }

    public List<string>? Hobbies { get; set; } = new();


    public static string GenerateGravatarUrl(UserId userId) =>
        // Logic to create Gravatar URL based on user's unique identifier
        $"https://www.gravatar.com/avatar/{userId}?d=identicon";

    public override bool Equals(object? obj) => obj is User user && Equals(user);

    public override int GetHashCode() => Id.GetHashCode();
}
