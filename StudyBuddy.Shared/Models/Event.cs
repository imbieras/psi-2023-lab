using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Shared.Models;

public class Event
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string Text { get; set; }
    public string? Color { get; set; }
}
