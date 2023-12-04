using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Attributes;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Controllers;

public class SchedulingController : Controller
{
    private readonly IUserSessionService _userSessionService;

    public SchedulingController(IUserSessionService userSessionService)
    {
        _userSessionService = userSessionService;
    }

    [CustomAuthorize]
    public IActionResult Index()
    {
        UserId? userId = _userSessionService.GetCurrentUserId();

        Guid guid = (Guid)userId;

        ViewBag.UserId = guid;
        return View();
    }
}
