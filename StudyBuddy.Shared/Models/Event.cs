using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Shared.Models;

public class Event
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime _start;
    public DateTime Start
    {
        get => _start.ToLocalTime();
        set => _start = value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();
    }

    private DateTime _end;
    public DateTime End
    {
        get => _end.ToLocalTime();
        set => _end = value.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
            : value.ToUniversalTime();
    }

    public string Text { get; set; }
    public string? Color { get; set; }
}
