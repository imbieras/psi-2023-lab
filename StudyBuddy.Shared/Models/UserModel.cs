using System.ComponentModel.DataAnnotations;
using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Shared.Models;

public class User : IUser, IEquatable<User>
{
    private User()
    {
    }

    public User(UserId id, string username, string passwordHash, UserFlags flags, UserTraits traits,
        List<string>? hobbies)
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        Flags = flags;
        Traits = traits;
        Hobbies = hobbies;
    }

    public UserId Id { get; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserFlags Flags { get; set; }

    public UserTraits Traits { get; set; } = new();

    public List<string>? Hobbies { get; set; } = new();

    public bool Equals(User? other)
    {
        if (other == null)
        {
            return false;
        }

        return Id == other.Id;
    }

    public bool Equals(IUser? other)
    {
        if (other == null)
        {
            return false;
        }

        return Id == other.Id;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            yield return new ValidationResult("Name cannot be empty", new[] { nameof(Username) });
        }

        if (string.IsNullOrWhiteSpace(PasswordHash))
        {
            yield return new ValidationResult("Password hash cannot be empty", new[] { nameof(PasswordHash) });
        }

        if ((Flags & UserFlags.Inactive) != 0 || (Flags & UserFlags.Admin) != 0)
        {
            yield return new ValidationResult("User cannot be inactive or admin", new[] { nameof(Flags) });
        }

        if (Traits.Birthdate > DateTime.UtcNow)
        {
            yield return new ValidationResult("Birthdate cannot be in the future", new[] { "Traits.Birthdate" });
        }

        if (string.IsNullOrWhiteSpace(Traits.Subject))
        {
            yield return new ValidationResult("Subject cannot be empty", new[] { "Traits.Subject" });
        }
    }


    public static string GenerateGravatarUrl(UserId userId) =>
        // Logic to create Gravatar URL based on user's unique identifier
        $"https://www.gravatar.com/avatar/{userId}?d=identicon";

    public override bool Equals(object? obj) => obj is User user && Equals(user);

    public override int GetHashCode() => Id.GetHashCode();
}
