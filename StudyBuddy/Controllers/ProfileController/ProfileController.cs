using Markdig;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Managers.UserManager;
using StudyBuddy.Models;
using StudyBuddy.Services;
using StudyBuddy.Services.UserService;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Controllers.ProfileController;

public class ProfileController : Controller
{
    private readonly IUserManager _userManager;
    private readonly IUserService _userService;

    public ProfileController(IUserManager userManager, IUserService userService)
    {
        _userManager = userManager;
        _userService = userService;
    }

    public IActionResult DisplayProfiles([FromQuery] ProfileFilterModel filterModel)
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


            /* Year filter */
            if (filterModel.StartYear == 0)
            {
                filterModel.StartYear = 1900;
            }

            if (filterModel.EndYear == 0)
            {
                filterModel.EndYear = DateTime.Now.Year;
            }

            if (filterModel.StartYear != 0 && filterModel.EndYear != 0)
            {
                userList = GenericFilterService<IUser>.FilterByPredicate(userList,
                    u => u.Traits.Birthdate.Year >= filterModel.StartYear &&
                         u.Traits.Birthdate.Year <= filterModel.EndYear);
            }

            /* Subject filter */
            if (!string.IsNullOrEmpty(filterModel.Subject))
            {
                // Input is assumed to be safe since it's coming from the model property
                userList = GenericFilterService<IUser>.FilterByPredicate(userList,
                    u => u.Traits.Subject.Equals(filterModel.Subject));
            }

            return View(userList);
        }
        catch (Exception ex)
        {
            // Log the exception
            ViewBag.ErrorMessage = "An error occurred while retrieving user profiles. " + ex.Message;
            return View(new List<IUser>()); // Provide an empty list
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
                return View("ViewFullProfile", user);
            }
        }

        ErrorViewModel errorModel = new() { ErrorMessage = "User not found or invalid ID." };
        return View("Error", errorModel);
    }

    public IActionResult CreateProfile() => View();

    [HttpPost]
    public async Task<IActionResult> SaveProfile(ProfileDTO profileDTO)
    {
        const UserFlags flags = UserFlags.Registered;

        try
        {
            string avatarPath = string.Empty;

            if (profileDTO.Avatar is { Length: > 0 })
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");

                // Check if the "avatars" folder exists, and create it if it doesn't
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid() + "_" + profileDTO.Avatar.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await using FileStream fileStream = new(filePath, FileMode.Create);
                await profileDTO.Avatar.CopyToAsync(fileStream);

                avatarPath = uniqueFileName;
            }

            string htmlContent = string.Empty;
            if (profileDTO.MarkdownContent != null)
            {
                // Convert Markdown to HTML
                htmlContent = Markdown.ToHtml(profileDTO.MarkdownContent);
            }

            Coordinates? location = null;
            if (double.TryParse(profileDTO.Longitude, out double parsedLongitude) &&
                double.TryParse(profileDTO.Latitude, out double parsedLatitude))
            {
                location = Coordinates.From((parsedLongitude, parsedLatitude));
            }

            UserTraits traits = new()
            {
                Birthdate = DateTime.Parse(profileDTO.Birthdate),
                Subject = profileDTO.Subject,
                AvatarPath = avatarPath,
                Description = htmlContent,
                Hobbies = profileDTO.Hobbies
            };

            if (location != null)
            {
                traits.Location = location.Value;
            }

            _userManager.RegisterUser(profileDTO.Name, flags, traits);

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
            return RedirectToAction("Index", controllerName: "Home");
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

        Response.Cookies.Append(key: "UserId", value: userId, cookieOptions);

        return RedirectToAction("Index", controllerName: "Home");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("UserId");

        return RedirectToAction("Index", controllerName: "Home");
    }
}
