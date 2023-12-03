using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Shared.Abstractions;

public interface IUser : IEquatable<IUser>, IValidatableObject
{
    [Required] UserId Id { get; }
    [Required] string Username { get; set; }

    [Required] string PasswordHash { get; set; }

    [Required] UserFlags Flags { get; set; }

    [Required] UserTraits Traits { get; set; }

    List<string>? Hobbies { get; set; }
}
