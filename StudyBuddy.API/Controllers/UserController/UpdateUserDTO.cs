﻿using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Controllers.UserController;

public class UpdateUserDto
{
    public UserId Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public UserFlags Flags { get; set; }
    public UserTraits Traits { get; set; } = new();
    public List<string>? Hobbies { get; set; } = new();
}