using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;
using Markdig;

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
    public async Task<IActionResult> SaveProfile(string name, string birthdate, string subject, IFormFile? avatar, string? markdownContent, List<string> hobbies, string? longitude, string? latitude)
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

            Coordinates? location = null;
            if (double.TryParse(longitude, out double parsedLongitude) && double.TryParse(latitude, out double parsedLatitude))
                location = Coordinates.From((parsedLongitude, parsedLatitude));

            UserTraits traits = new()
            {
                Birthdate = DateTime.Parse(birthdate),
                Subject = subject,
                AvatarPath = avatarPath,
                Description = htmlContent,
                Hobbies = hobbies
            };

            if (location != null)
                traits.Location = location.Value;

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
    public IActionResult MatchUsers(string currentUser, string otherUser)
    {
        try
        {
            // Convert user IDs to the appropriate type (e.g., Guid)
            UserId currentUserId = UserId.From(Guid.Parse(currentUser));
            UserId otherUserId = UserId.From(Guid.Parse(otherUser));

            // Call UserManager.MatchUsers with the user IDs
            _matchingManager.MatchUsers(currentUserId, otherUserId);

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

        // Redirect back to the explore page
        return RedirectToAction("DisplayProfiles");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("UserId");

        return RedirectToAction("Index", "Home");
    }
}
