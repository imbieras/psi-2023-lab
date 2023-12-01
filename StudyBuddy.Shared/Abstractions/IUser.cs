using System.ComponentModel.DataAnnotations;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Shared.Abstractions;

public interface IUser : IEquatable<IUser>, IValidatableObject
{
    UserId Id { get; }

    string Username { get; set; }

    string PasswordHash { get; set; }

    UserFlags Flags { get; set; }

    UserTraits Traits { get; set; }

    List<string>? Hobbies { get; set; }
}
