using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;
using Markdig;
using System.Collections.Specialized;

namespace StudyBuddy.Controllers;

public class ProfileController : Controller
{
    private readonly IUserManager _userManager;
    private readonly IMatchingManager _matchingManager;
    private readonly IUserService _userService;

    public ProfileController(IUserManager userManager, IMatchingManager matchingManager, IUserService userService)
    {
        _userManager = userManager;
        _matchingManager = matchingManager;
        _userService = userService;
    }

    public IActionResult DisplayProfiles()
    {
        try
        {
            // Get the current user's ID from UserService if not null, otherwise use a default value

            UserId? currentUserId = _userService.GetCurrentUserId();

            List<IUser> userList = _userManager.GetAllUsers();

            // Pass the current user's ID to the view
            if (currentUserId != null)
            {
                ViewBag.CurrentUserId = currentUserId;
            }

            return View(userList);
        }
        catch (Exception ex)
        {
            // Log the exception
            ViewBag.ErrorMessage = "An error occurred while retrieving user profiles. " + ex.Message;
            return View(new List<IUser>());// Provide an empty list
        }
    }

    public IActionResult UserProfile(string id)
    {
        if (Guid.TryParse(id, out Guid parsedGuid))
        {
            UserId parseUserId = UserId.From(parsedGuid);

            IUser? user = _userManager.GetUserById(parseUserId);

            if (user != null)
            {
                return View("DisplayProfiles", new List<IUser?>() { user });
            }
        }

        ErrorViewModel errorModel = new() { ErrorMessage = "User not found or invalid ID." };
        return View("Error", errorModel);
    }

    public IActionResult CreateProfile() => View();

    [HttpPost]
    public async Task<IActionResult> SaveProfile(string name, string birthdate, string subject, IFormFile? avatar, string? markdownContent, List<string> hobbies)
    {
        const UserFlags flags = UserFlags.Registered;

        try
        {
            string avatarPath = string.Empty;

            if (avatar is { Length: > 0 })
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");

                // Check if the "avatars" folder exists, and create it if it doesn't
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid() + "_" + avatar.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await using FileStream fileStream = new(filePath, FileMode.Create);
                await avatar.CopyToAsync(fileStream);

                avatarPath = uniqueFileName;
            }

            string htmlContent = string.Empty;
            if (markdownContent != null)
            {
                // Convert Markdown to HTML
                htmlContent = Markdown.ToHtml(markdownContent);
            }

            UserTraits traits = new()
            {
                Birthdate = DateTime.Parse(birthdate),
                Subject = subject,
                AvatarPath = avatarPath,
                Description = htmlContent,
                Hobbies = hobbies
            };

            _userManager.RegisterUser(name, flags, traits);

            TempData["SuccessMessage"] = "Profile created successfully";

            return RedirectToAction("CreateProfile");
        }
        catch (Exception ex)
        {
            ErrorViewModel errorModel = new() { ErrorMessage = "Error uploading file: " + ex.Message };
            return View("Error", errorModel);
        }
    }

    public IActionResult CurrentRandomUserProfile()
    {
        UserId? currentUserId = _userService.GetCurrentUserId();
        if (Guid.TryParse(currentUserId.ToString(), out Guid userIdGuid))
        {
            UserId parseUserId = UserId.From(userIdGuid);

            IUser? currentUser = _userManager.GetUserById(parseUserId);

            IUser? currentRandomUser = _userManager.GetCurrentRandomUser(currentUser);

            return View("RandomProfile", currentRandomUser);
        }
        return View("Error");
    }

    public IActionResult RandomProfile()
    {
        // Pass the current user's ID to the view
        UserId? currentUserId = _userService.GetCurrentUserId();

        if (currentUserId != null)
        {
            ViewBag.CurrentUserId = currentUserId;
        }


        if (Guid.TryParse(currentUserId.ToString(), out Guid userIdGuid))
        {
            UserId parseUserId = UserId.From(userIdGuid);

            IUser? currentUser = _userManager.GetUserById(parseUserId);


            if (currentUser == null)
            {
                return View("Login");
            }

            IUser? randomUser = null;
            int counter = 0;

            // Keep generating random users until an unmatched and unrequested user is found
            while (randomUser == null || (_matchingManager.IsMatched(currentUser.Id, randomUser.Id) && _matchingManager.IsRequestedMatch(currentUser.Id, randomUser.Id)))
            {
                randomUser = _userManager.GetRandomUser(currentUser);

                counter++;
                if (counter > _userManager.GetAllUsers().Count())
                    break;
            }
            TempData.Remove("HideGoBackButton");


            return View("RandomProfile", randomUser);
        }
        else
        {
            View("Error");
        }
        return View("Login");
    }

    public IActionResult PreviousProfile()
    {
        // Pass the current user's ID to the view
        UserId? currentUserId = _userService.GetCurrentUserId();

        if (currentUserId != null)
        {
            ViewBag.CurrentUserId = currentUserId;
        }

        if (Guid.TryParse(currentUserId.ToString(), out Guid userIdGuid))
        {
            UserId parseUserId = UserId.From(userIdGuid);

            IUser? currentUser = _userManager.GetUserById(parseUserId);

            if (currentUser == null)
            {
                return View("Login");
            }

            IUser? previousUser = _userManager.GetPreviousRandomProfile(currentUser);

            TempData["HideGoBackButton"] = true;

            return View("RandomProfile", previousUser);

        }
        else
        {
            View("Error");
        }


        return View("Login");
    }



    public IActionResult Login(string? userId)
    {
        UserId? currentUserId = _userService.GetCurrentUserId();

        if (currentUserId != null && _userManager.GetUserById(currentUserId.Value) != null)
        {
            return RedirectToAction("Index", "Home");
        }

        if (string.IsNullOrEmpty(userId))
        {
            return View(userId);
        }

        if (!Guid.TryParse(userId, out Guid userIdGuid))
        {
            TempData["ErrorMessage"] = "Invalid user ID format";
            return RedirectToAction("Login");
        }

        UserId parseUserId = UserId.From(userIdGuid);

        IUser? user = _userManager.GetUserById(parseUserId);

        if (user == null)
        {
            TempData["ErrorMessage"] = "No such user found";
            return RedirectToAction("Login");
        }

        CookieOptions cookieOptions = new()
        {
            Expires = DateTime.Now.AddHours(1),
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Strict,
            Secure = true
        };

        Response.Cookies.Append("UserId", userId, cookieOptions);

        return RedirectToAction("Index", "Home");
    }


    [HttpPost]
    public IActionResult MatchUsers(string currentUser, string otherUser, string redirectAction)
    {
        try
        {
            // Convert user IDs to the appropriate type (e.g., Guid)
            UserId currentUserId = UserId.From(Guid.Parse(currentUser));
            UserId otherUserId = UserId.From(Guid.Parse(otherUser));

            // Call UserManager.MatchUsers with the user IDs
            _matchingManager.MatchUsers(currentUserId, otherUserId);

            if (_matchingManager.IsRequestedMatch(currentUserId, otherUserId))
            {
                // Store a "Match request sent" message in TempData
                TempData["MatchRequestSentMessage"] = "Match request sent.";
            }

            if (_matchingManager.IsMatched(currentUserId, otherUserId))
            {
                TempData["SuccessMessage"] = "Users matched successfully";
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            TempData["ErrorMessage"] = "An error occurred while matching users. " + ex.Message;
        }

        if (redirectAction == "RandomProfile")
        {
            return RedirectToAction("RandomProfile");
        }
        else if (redirectAction == "DisplayProfiles")
        {
            return RedirectToAction("DisplayProfiles");
        }
        else if (redirectAction == "CurrentRandomUserProfile")
        {
            return RedirectToAction("CurrentRandomUserProfile");
        }

        else
        {
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("UserId");

        return RedirectToAction("Index", "Home");
    }
}
