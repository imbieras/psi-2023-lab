using Markdig;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Attributes;
using StudyBuddy.Models;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.Exceptions;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Controllers.ProfileController;

public class ProfileController : Controller
{
    private readonly ILogger<ProfileController> _logger;
    private readonly IUserSessionService _userSessionService;
    private readonly IHttpClientFactory _clientFactory;

    public ProfileController(ILogger<ProfileController> logger, IUserSessionService userSessionService,
        IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _userSessionService = userSessionService;
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> UserProfile(string id)
    {
        if (Guid.TryParse(id, out Guid parsedGuid))
        {
            UserId parseUserId = UserId.From(parsedGuid);

            var httpClient = _clientFactory.CreateClient("StudyBuddy.API");
            var responseUser = await httpClient.GetAsync($"api/v1/user/{parseUserId}");
            responseUser.EnsureSuccessStatusCode();

            var user = await responseUser.Content.ReadFromJsonAsync<IUser>();

            if (user != null)
            {
                var responseUserHobbies = await httpClient.GetAsync($"api/v1/user/{parseUserId}/hobbies");
                responseUserHobbies.EnsureSuccessStatusCode();

                user.Hobbies = await responseUserHobbies.Content.ReadFromJsonAsync<List<string>>();

                return View("ViewFullProfile", user);
            }
        }

        ErrorViewModel errorModel = new() { ErrorMessage = "User not found or invalid ID." };
        return View("Error", errorModel);
    }

    public IActionResult CreateProfile()
    {
        // Don't let the user access this page if they are already logged in
        if (_userSessionService.GetCurrentUserId() != null)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SaveProfile(ProfileDto profileDto)
    {
        var httpClient = _clientFactory.CreateClient("StudyBuddy.API");
        var responseUser = await httpClient.GetAsync($"api/v1/user/{profileDto.Name}");
        responseUser.EnsureSuccessStatusCode();

        var existingUser = await responseUser.Content.ReadFromJsonAsync<IUser>();
        if (existingUser != null)
        {
            TempData["ErrorMessage"] = "Username is already taken..";
            return View("CreateProfile", profileDto);
        }

        if (profileDto.Password != profileDto.ConfirmPassword)
        {
            TempData["ErrorMessage"] = "The password and confirmation password do not match.";
            return View("CreateProfile", profileDto);
        }

        const UserFlags flags = UserFlags.Registered;

        string htmlContent = ConvertMarkdownToHtml(profileDto.MarkdownContent);
        UserTraits traits = CreateUserTraits(profileDto, htmlContent);

        try
        {
            var responseRegisterUser = await httpClient.PostAsJsonAsync("api/v1/user",
                new
                {
                    profileDto.Name,
                    profileDto.Password,
                    Flags = flags,
                    Traits = traits,
                    profileDto.Hobbies
                });

            responseRegisterUser.EnsureSuccessStatusCode();
            _logger.LogInformation("User registered successfully: {UserName}", profileDto.Name);
        }
        catch (InvalidPasswordException)
        {
            // ("{8,}")
            _logger.LogWarning("Invalid password format for user: {UserName}", profileDto.Name);
            TempData["ErrorMessage"] = "Invalid password format. Password must be at least 8 characters long.";
            return View("CreateProfile", profileDto);
        }
        catch (InvalidUsernameException)
        {
            // ("^[A-Za-z0-9]+([A-Za-z0-9]*|[._-]?[A-Za-z0-9]+)*$")
            _logger.LogWarning("Invalid username format for user: {UserName}", profileDto.Name);
            TempData["ErrorMessage"] = "Invalid username format. Username must be alphanumeric and can contain . _ -";
            return View("CreateProfile", profileDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving profile for user: {UserName}", profileDto.Name);
            TempData["ErrorMessage"] = "Error saving profile: " + ex.Message;
            return View("CreateProfile", profileDto);
        }

        _logger.LogInformation("Profile created successfully for user: {UserName}", profileDto.Name);
        TempData["SuccessMessage"] = "Profile created successfully";
        return RedirectToAction("CreateProfile");
    }

    private static string ConvertMarkdownToHtml(string? markdownContent) =>
        markdownContent != null ? Markdown.ToHtml(markdownContent) : string.Empty;

    private static UserTraits CreateUserTraits(ProfileDto profileDto, string htmlContent)
    {
        UserTraits traits = new()
        {
            Birthdate = DateTime.Parse(profileDto.Birthdate).ToUniversalTime(),
            Subject = profileDto.Subject,
            Description = htmlContent
        };

        if (!double.TryParse(profileDto.Longitude, out double longitude) ||
            !double.TryParse(profileDto.Latitude, out double latitude))
        {
            return traits;
        }

        traits.Longitude = longitude;
        traits.Latitude = latitude;

        return traits;
    }

    public async Task<IActionResult> Login(string? username, string? password)
    {
        // Check if the user is already logged in
        UserId? currentUserId = _userSessionService.GetCurrentUserId();

        var httpClient = _clientFactory.CreateClient("StudyBuddy.API");

        var responseUser = await httpClient.GetAsync($"api/v1/user/{currentUserId}");
        responseUser.EnsureSuccessStatusCode();

        var user = await responseUser.Content.ReadFromJsonAsync<IUser>();

        if (currentUserId != null && user != null)
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

        return RedirectToAction("RandomProfile", "Matching");
    }

    [CustomAuthorize]
    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("UserId");

        return RedirectToAction("Index", "Home");
    }

    [CustomAuthorize]
    public async Task<IActionResult> EditProfile()
    {
        UserId currentUserId = (UserId)_userSessionService.GetCurrentUserId()!;

        var httpClient = _clientFactory.CreateClient("StudyBuddy.API");
        var responseCurrentUser = await httpClient.GetAsync($"api/v1/user/{currentUserId}");
        responseCurrentUser.EnsureSuccessStatusCode();

        var user = await responseCurrentUser.Content.ReadFromJsonAsync<IUser>();

        return View(user);
    }

    [CustomAuthorize]
    public async Task<IActionResult> UpdateProfile(ProfileDto profileDto)
    {
        UserId currentUserId = (UserId)_userSessionService.GetCurrentUserId()!;

        var httpClient = _clientFactory.CreateClient("StudyBuddy.API");
        var responseCurrentUser = await httpClient.GetAsync($"api/v1/user/{currentUserId}");
        responseCurrentUser.EnsureSuccessStatusCode();

        var user = await responseCurrentUser.Content.ReadFromJsonAsync<IUser>();

        user.Traits.Subject = profileDto.Subject;
        user.Hobbies = profileDto.Hobbies;
        user.Traits.Description = ConvertMarkdownToHtml(profileDto.MarkdownContent);

        var responseUpdateUser = await httpClient.PutAsJsonAsync($"api/v1/user/{currentUserId}", user);
        responseUpdateUser.EnsureSuccessStatusCode();

        TempData["SuccessMessage"] = "Profile updated successfully";
        return RedirectToAction("EditProfile");
    }
}
