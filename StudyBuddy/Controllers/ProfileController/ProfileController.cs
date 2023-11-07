using Markdig;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.Services;
using StudyBuddy.Services.UserService;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Controllers.ProfileController;

public class ProfileController : Controller
{
    private readonly IUserSessionService _userSessionService;
    private readonly IUserService _userService;

    public ProfileController(IUserService userService, IUserSessionService userSessionService)
    {
        _userService = userService;
        _userSessionService = userSessionService;
    }

    public async Task<IActionResult> DisplayProfiles([FromQuery] ProfileFilterModel filterModel)
    {
        try
        {
            // Get the current user's ID from UserService if not null, otherwise use a default value
            UserId? currentUserId = _userSessionService.GetCurrentUserId();

            List<IUser> userList = (await _userService.GetAllUsersAsync()).ToList();

            // Pass the current user's ID to the view
            if (currentUserId != null)
            {
                ViewBag.CurrentUserId = currentUserId;
            }


            // Year filter
            if (filterModel.StartYear == 0)
            {
                filterModel.StartYear = 1900;
            }

            if (filterModel.EndYear == 0)
            {
                filterModel.EndYear = DateTime.Now.Year;
            }

            userList = GenericFilterService<IUser>.FilterByPredicate(userList,
            u => u.Traits.Birthdate.Year >= filterModel.StartYear &&
                 u.Traits.Birthdate.Year <= filterModel.EndYear);

            // Subject filter
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
            return View(new List<IUser>());// Provide an empty list
        }
    }


    public async Task<IActionResult> UserProfile(string id)
    {
        if (Guid.TryParse(id, out Guid parsedGuid))
        {
            UserId parseUserId = UserId.From(parsedGuid);

            IUser? user = await _userService.GetUserByIdAsync(parseUserId);
            user.Hobbies = await _userService.GetHobbiesById(parseUserId);

            return View("ViewFullProfile", user);
        }

        ErrorViewModel errorModel = new() { ErrorMessage = "User not found or invalid ID." };
        return View("Error", errorModel);
    }

    public IActionResult CreateProfile() => View();

    [HttpPost]
    public async Task<IActionResult> SaveProfile(ProfileDto profileDto)
    {
        const UserFlags flags = UserFlags.Registered;

        try
        {
            string avatarPath = string.Empty;

            if (profileDto.Avatar is { Length: > 0 })
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");

                // Check if the "avatars" folder exists, and create it if it doesn't
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid() + "_" + profileDto.Avatar.FileName;

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await using FileStream fileStream = new(filePath, FileMode.Create);
                await profileDto.Avatar.CopyToAsync(fileStream);

                avatarPath = uniqueFileName;
            }

            string htmlContent = string.Empty;
            if (profileDto.MarkdownContent != null)
            {
                // Convert Markdown to HTML
                htmlContent = Markdown.ToHtml(profileDto.MarkdownContent);
            }

            UserTraits traits = new()
            {
                Birthdate = DateTime.Parse(profileDto.Birthdate).ToUniversalTime(),
                Subject = profileDto.Subject,
                AvatarPath = avatarPath,
                Description = htmlContent
            };

            if (double.TryParse(profileDto.Longitude, out double parsedLongitude) && double.TryParse(profileDto.Latitude, out double parsedLatitude))
            {
                traits.Longitude = parsedLongitude;
                traits.Latitude = parsedLatitude;
            }

            await _userService.RegisterUserAsync(profileDto.Name, flags, traits, profileDto.Hobbies);

            TempData["SuccessMessage"] = "Profile created successfully";

            return RedirectToAction("CreateProfile");
        }
        catch (Exception ex)
        {
            ErrorViewModel errorModel = new() { ErrorMessage = "Error uploading file: " + ex.Message };
            return View("Error", errorModel);
        }
    }

    public async Task<IActionResult> Login(string? userId)
    {
        UserId? currentUserId = _userSessionService.GetCurrentUserId();

        if (currentUserId != null && await _userService.GetUserByIdAsync(currentUserId.Value) != null)
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

        IUser? user = await _userService.GetUserByIdAsync(parseUserId);

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
    public IActionResult Logout()
    {
        Response.Cookies.Delete("UserId");

        return RedirectToAction("Index", "Home");
    }
}
