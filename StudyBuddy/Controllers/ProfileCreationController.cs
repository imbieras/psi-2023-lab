using Microsoft.AspNetCore.Mvc;

namespace StudyBuddy.Controllers;

public class ProfileCreationController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ProcessNewProfile(string name, string birthdate, string subject)
    {
        // TODO: Switch to view model to pass data to view
        ViewData["name"] = name;
        ViewData["birthdate"] = birthdate;
        ViewData["subject"] = subject;

        return View();
    }
}
