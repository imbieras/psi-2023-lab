using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.Models;

namespace StudyBuddy.API.Controllers.UserController;

public class RegisterUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserFlags Flags { get; set; }
    public UserTraits Traits { get; set; } = new();
    public List<string>? Hobbies { get; set; } = new();
}
