namespace StudyBuddy.Abstractions;

[Flags]
public enum UserFlags : byte
{
    Inactive = 0x0,
    Unregistered = 0x2,
    Registered = 0x4,
    Banned = 0x8,

    Admin = 0x64
}
