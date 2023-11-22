using System.ComponentModel.DataAnnotations;
using StudyBuddy.Abstractions;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class User : IUser, IEquatable<User>, IValidatableObject
{
    private User() {}

    public User(UserId id, string name, string passwordHash, UserFlags flags, UserTraits traits, List<string>? hobbies)
    {
        Id = id;
        Name = name;
        PasswordHash = passwordHash;
        Flags = flags;
        Traits = traits;
        Hobbies = hobbies;
    }

    public UserId Id { get; }

    public string Name { get; set; } = string.Empty;

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
        if (string.IsNullOrWhiteSpace(this.Name))
        {
            yield return new ValidationResult("Name cannot be empty", new[] { nameof(this.Name) });
        }

        if (string.IsNullOrWhiteSpace(this.PasswordHash))
        {
            yield return new ValidationResult("Password hash cannot be empty", new[] { nameof(this.PasswordHash) });
        }

        if ((this.Flags & UserFlags.Inactive) != 0 || (this.Flags & UserFlags.Admin) != 0)
        {
            yield return new ValidationResult("User cannot be inactive or admin", new[] { nameof(this.Flags) });
        }

        if (this.Traits.Birthdate > DateTime.UtcNow)
        {
            yield return new ValidationResult("Birthdate cannot be in the future", new[] { "Traits.Birthdate" });
        }

        if (string.IsNullOrWhiteSpace(this.Traits.Subject))
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
