using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Attributes;
using StudyBuddy.Services.MatchingService;
using StudyBuddy.Services.UserService;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Controllers;

[CustomAuthorize]
public class MatchingController : Controller
{
    private const string LoginPath = "~/Views/Profile/Login.cshtml";
    private readonly IMatchingService _matchingService;
    private readonly IUserService _userService;
    private readonly IUserSessionService _userSessionService;
    private readonly ILogger<MatchingController> _logger;

    public MatchingController(IUserService userService, IMatchingService matchingService,
        IUserSessionService userSessionService, ILogger<MatchingController> logger)
    {
        _userService = userService;
        _matchingService = matchingService;
        _userSessionService = userSessionService;
        _logger = logger;
    }

    public async Task<IActionResult> CurrentRandomUserProfile()
    {
        UserId currentUserId = (UserId)_userSessionService.GetCurrentUserId()!;

        IUser currentUser = (await _userService.GetUserByIdAsync(currentUserId))!;

        IUser? currentRandomUser = await _userService.GetUltimateSeenUserAsync(currentUser.Id);
        ViewBag.ShowMatchRequestMessage = true;

        return View("RandomProfile", currentRandomUser);
    }

    public async Task<IActionResult> RandomProfile()
    {
        UserId currentUserId = (UserId)_userSessionService.GetCurrentUserId()!;

        ViewBag.CurrentUserId = currentUserId;
        ViewBag.ShowMatchRequestMessage = false;

        IUser currentUser = (await _userService.GetUserByIdAsync(currentUserId))!;

        // Prepare ViewBag for 'Go back!' button
        ViewBag.ViewedFirstProfile = await _userService.IsUserNotSeenAnyUserAsync(currentUser.Id);

        var allUsers = (await _userService.GetAllUsersAsync()).Where(u => u.Id != currentUser.Id).ToList();

        var unseenUsers = allUsers.Where(u => !_userService.IsUserSeenAsync(currentUser.Id, u.Id).Result).ToList();

        IUser? randomUser = null;

        if (unseenUsers.Any())
        {
            Random rnd = new();
            randomUser = unseenUsers[rnd.Next(unseenUsers.Count)];
            await _userService.UserSeenAsync(currentUser.Id, randomUser.Id);
        }

        TempData.Remove("HideGoBackButton");
        return View("RandomProfile", randomUser);
    }

    public IActionResult SetViewedFirstProfile()
    {
        ViewBag.ViewedFirstProfile = true;
        return RedirectToAction("RandomProfile");
    }

    public async Task<IActionResult> PenultimateProfile()
    {
        // Pass the current user's ID to the view
        UserId currentUserId = (UserId)_userSessionService.GetCurrentUserId()!;

        ViewBag.CurrentUserId = currentUserId;
        ViewBag.ShowMatchRequestMessage = false;

        IUser currentUser = (await _userService.GetUserByIdAsync(currentUserId))!;

        IUser? previousUser = await _userService.GetPenultimateSeenUserAsync(currentUser.Id);

        TempData["HideGoBackButton"] = true;

        return View("RandomProfile", previousUser);
    }


    public async Task<IActionResult> UltimateProfile()
    {
        // Pass the current user's ID to the view
        UserId currentUserId = (UserId)_userSessionService.GetCurrentUserId()!;

        ViewBag.CurrentUserId = currentUserId;
        ViewBag.ShowMatchRequestMessage = false;

        IUser currentUser = (await _userService.GetUserByIdAsync(currentUserId))!;

        IUser? previousUser = await _userService.GetUltimateSeenUserAsync(currentUser.Id);

        TempData["HideGoBackButton"] = true;

        return View("RandomProfile", previousUser);
    }


    [HttpPost]
    public async Task<IActionResult> MatchUsers(string currentUser, string otherUser, string redirectAction)
    {
        try
        {
            // Convert user IDs to the appropriate type (e.g., Guid)
            UserId currentUserId = UserId.From(Guid.Parse(currentUser));
            UserId otherUserId = UserId.From(Guid.Parse(otherUser));

            // Call UserManager.MatchUsers with the user IDs
            await _matchingService.MatchUsersAsync(currentUserId, otherUserId);

            if (await _matchingService.IsMatchedAsync(currentUserId, otherUserId))
            {
                TempData["SuccessMessage"] = "Users matched successfully";
                ViewBag.ShowMatchRequestMessage = false;
            }
            else
            {
                // Store a "Match request sent" message in TempData
                TempData["MatchRequestSentMessage"] = "Match request sent.";
                ViewBag.ShowMatchRequestMessage = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while matching user {currentUser} to {otherUser}");
            TempData["ErrorMessage"] = "An error occurred while matching users. " + ex.Message;
        }

        return redirectAction switch
        {
            "RandomProfile" => RedirectToAction("RandomProfile", "Matching"),
            "CurrentRandomUserProfile" => RedirectToAction("CurrentRandomUserProfile", "Matching"),
            _ => RedirectToAction("Index", "Home")
        };
    }
}
