using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Services.MatchingService;
using StudyBuddy.Services.UserService;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Controllers;

public class MatchingController : Controller
{
    private const string LoginPath = "~/Views/Profile/Login.cshtml";
    private readonly IMatchingService _matchingService;
    private readonly IUserService _userService;
    private readonly IUserSessionService _userSessionService;

    public MatchingController(IUserService userService, IMatchingService matchingService, IUserSessionService userSessionService)
    {
        _userService = userService;
        _matchingService = matchingService;
        _userSessionService = userSessionService;
    }

    public async Task<IActionResult> CurrentRandomUserProfile()
    {
        UserId? currentUserId = _userSessionService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserId.ToString(), out Guid userIdGuid))
        {
            return View(LoginPath);
        }

        UserId parseUserId = UserId.From(userIdGuid);

        IUser? currentUser = await _userService.GetUserByIdAsync(parseUserId);

        if (currentUser == null)
        {
            return View(LoginPath);
        }

        IUser? currentRandomUser = _userService.GetCurrentRandomUser(currentUser);
        ViewBag.ShowMatchRequestMessage = true;

        return View("RandomProfile", currentRandomUser);
    }

    public async Task<IActionResult> RandomProfile()
    {
        // Pass the current user's ID to the view
        UserId? currentUserId = _userSessionService.GetCurrentUserId();

        if (currentUserId != null)
        {
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.ShowMatchRequestMessage = false;
        }

        if (!Guid.TryParse(currentUserId.ToString(), out Guid userIdGuid))
        {
            return View(LoginPath);
        }

        UserId parseUserId = UserId.From(userIdGuid);

        IUser? currentUser = await _userService.GetUserByIdAsync(parseUserId);

        ViewBag.ViewedFirstProfile =
            currentUser != null && _userService.IsUsedIndexesEmpty(currentUser);// For 'Go back!' button

        if (currentUser == null)
        {
            return View(LoginPath);
        }

        IUser? randomUser = null;
        int counter = 0;

        // Keep generating random users until an unmatched and unrequested user is found
        while (randomUser == null || (await _matchingService.IsMatchedAsync(currentUser.Id, randomUser.Id) &&
                                      await _matchingService.IsRequestedMatchAsync(currentUser.Id, randomUser.Id)))
        {
            randomUser = _userService.GetRandomUser(currentUser);

            counter++;
            if (counter > (await _userService.GetAllUsersAsync()).Count())
            {
                break;
            }
        }

        TempData.Remove("HideGoBackButton");

        return View("RandomProfile", randomUser);
    }

    public IActionResult SetViewedFirstProfile()
    {
        ViewBag.ViewedFirstProfile = true;
        return RedirectToAction("RandomProfile");
    }

    public async Task<IActionResult> PreviousProfile()
    {
        // Pass the current user's ID to the view
        UserId? currentUserId = _userSessionService.GetCurrentUserId();

        if (currentUserId != null)
        {
            ViewBag.CurrentUserId = currentUserId;
            ViewBag.ShowMatchRequestMessage = false;
        }

        if (!Guid.TryParse(currentUserId.ToString(), out Guid userIdGuid))
        {
            return View(LoginPath);
        }

        UserId parseUserId = UserId.From(userIdGuid);

        IUser? currentUser = await _userService.GetUserByIdAsync(parseUserId);

        if (currentUser == null)
        {
            return View(LoginPath);
        }

        IUser? previousUser = _userService.GetPreviousRandomProfile(currentUser);

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

            if (await _matchingService.IsRequestedMatchAsync(currentUserId, otherUserId))
            {
                // Store a "Match request sent" message in TempData
                TempData["MatchRequestSentMessage"] = "Match request sent.";
                ViewBag.ShowMatchRequestMessage = true;
            }

            if (await _matchingService.IsMatchedAsync(currentUserId, otherUserId))
            {
                TempData["SuccessMessage"] = "Users matched successfully";
                ViewBag.ShowMatchRequestMessage = false;
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            TempData["ErrorMessage"] = "An error occurred while matching users. " + ex.Message;
        }

        return redirectAction switch
        {
            "RandomProfile" => RedirectToAction("RandomProfile", "Matching"),
            "DisplayProfiles" => RedirectToAction("DisplayProfiles", "Profile"),
            "CurrentRandomUserProfile" => RedirectToAction("CurrentRandomUserProfile", "Matching"),
            _ => RedirectToAction("Index", "Home")
        };
    }
}
