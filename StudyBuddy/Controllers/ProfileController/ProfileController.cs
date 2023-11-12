using Markdig;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Exceptions;
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
            return View(new List<IUser>()); // Provide an empty list
        }
    }


    public async Task<IActionResult> UserProfile(string id)
    {
        if (Guid.TryParse(id, out Guid parsedGuid))
        {
            UserId parseUserId = UserId.From(parsedGuid);

            IUser? user = await _userService.GetUserByIdAsync(parseUserId);
            if (user != null)
            {
                user.Hobbies = await _userService.GetHobbiesById(parseUserId);

                return View("ViewFullProfile", user);
            }
        }

        ErrorViewModel errorModel = new() { ErrorMessage = "User not found or invalid ID." };
        return View("Error", errorModel);
    }

    public IActionResult CreateProfile() => View();

    [HttpPost]
    public async Task<IActionResult> SaveProfile(ProfileDto profileDto)
    {
        const UserFlags flags = UserFlags.Registered;

        if (profileDto.Password != profileDto.ConfirmPassword)
        {
            TempData["ErrorMessage"] = "The password and confirmation password do not match.";
            return View("CreateProfile");
        }

        string avatarPath;
        try
        {
            avatarPath = await SaveAvatarAsync(profileDto.Avatar);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error saving avatar: " + ex.Message;
            return View("CreateProfile");
        }

        string htmlContent = ConvertMarkdownToHtml(profileDto.MarkdownContent);
        UserTraits traits = CreateUserTraits(profileDto, avatarPath, htmlContent);

        try
        {
            await _userService.RegisterUserAsync(profileDto.Name, profileDto.Password, flags, traits, profileDto.Hobbies);
        }
        catch (InvalidPasswordException ex)
        {
            // ("{8,}")
            TempData["ErrorMessage"] = "Invalid password format. Password must be at least 8 characters long.";
            return View("CreateProfile");
        }
        catch (InvalidUsernameException ex)
        {
            // ("^[A-Za-z0-9]+([A-Za-z0-9]*|[._-]?[A-Za-z0-9]+)*$")
            TempData["ErrorMessage"] = "Invalid username format. Username must be alphanumeric and can contain . _ -";
            return View("CreateProfile");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Error saving profile: " + ex.Message;
            return View("CreateProfile");
        }

        TempData["SuccessMessage"] = "Profile created successfully";
        return RedirectToAction("CreateProfile");
    }

    private async Task<string> SaveAvatarAsync(IFormFile? avatar)
    {
        if (avatar is null || avatar.Length == 0)
        {
            return string.Empty;
        }

        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "avatars");
        Directory.CreateDirectory(uploadsFolder);

        string uniqueFileName = Guid.NewGuid() + "_" + avatar.FileName;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using FileStream fileStream = new(filePath, FileMode.Create);
        await avatar.CopyToAsync(fileStream);

        return uniqueFileName;
    }

    private string ConvertMarkdownToHtml(string? markdownContent)
    {
        return markdownContent != null ? Markdown.ToHtml(markdownContent) : string.Empty;
    }

    private UserTraits CreateUserTraits(ProfileDto profileDto, string avatarPath, string htmlContent)
    {
        UserTraits traits = new()
        {
            Birthdate = DateTime.Parse(profileDto.Birthdate).ToUniversalTime(),
            Subject = profileDto.Subject,
            AvatarPath = avatarPath,
            Description = htmlContent
        };

        if (double.TryParse(profileDto.Longitude, out double longitude) &&
            double.TryParse(profileDto.Latitude, out double latitude))
        {
            traits.Longitude = longitude;
            traits.Latitude = latitude;
        }

        return traits;
    }

    public async Task<IActionResult> Login(string? username, string? password)
    {
        // Check if the user is already logged in
        UserId? currentUserId = _userSessionService.GetCurrentUserId();
        if (currentUserId != null && await _userService.GetUserByIdAsync(currentUserId.Value) != null)
        {
            return RedirectToAction("Index", "Home");
        }

        // Show the login view if the credentials are not provided
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return View();
        }

        // Authenticate the user
        if (!await _userSessionService.AuthenticateUser(username, password))
        {
            TempData["ErrorMessage"] = "Invalid username or password";
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
        Response.Cookies.Append("UserId", _userSessionService.GetCurrentUserId().ToString()!, cookieOptions);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("UserId");

        return RedirectToAction("Index", "Home");
    }
}
