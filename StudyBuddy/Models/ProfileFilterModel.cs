using Microsoft.AspNetCore.Mvc;

namespace StudyBuddy.Models;

public class ProfileFilterModel
{
    [BindProperty(Name = "startYear")]
    public int StartYear { get; set; }

    [BindProperty(Name = "endYear")]
    public int EndYear { get; set; }

    [BindProperty(Name = "subject")]
    public string? Subject { get; set; }
}
