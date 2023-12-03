using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Attributes;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.DTOs;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Controllers;

[CustomAuthorize]
public class MatchingController : Controller
{
    private readonly ILogger<MatchingController> _logger;
    private readonly IUserSessionService _userSessionService;
    private readonly IHttpClientFactory _clientFactory;

    public MatchingController(
        ILogger<MatchingController> logger,
        IUserSessionService userSessionService,
        IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _userSessionService = userSessionService;
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> CurrentRandomUserProfile()
    {
        UserId currentUserId = (UserId)_userSessionService.GetCurrentUserId()!;

        var httpClient = _clientFactory.CreateClient("StudyBuddy.API");
        var responseCurrentUser = await httpClient.GetAsync($"api/v1/user/{currentUserId}");
        responseCurrentUser.EnsureSuccessStatusCode();

        var currentUser = await responseCurrentUser.Content.ReadFromJsonAsync<User>();

        var responseUltimateUser = await httpClient.GetAsync($"api/v1/user/{currentUser.Id}/ultimate-seen-user");
        responseUltimateUser.EnsureSuccessStatusCode();

        var currentRandomUser = await responseUltimateUser.Content.ReadFromJsonAsync<User>();

        ViewBag.ShowMatchRequestMessage = true;

        return View("RandomProfile", currentRandomUser);
    }

    public async Task<IActionResult> RandomProfile()
    {
        UserId currentUserId = (UserId)_userSessionService.GetCurrentUserId()!;

        ViewBag.CurrentUserId = currentUserId;
        ViewBag.ShowMatchRequestMessage = false;

        var httpClient = _clientFactory.CreateClient("StudyBuddy.API");

        var responseCurrentUser = await httpClient.GetAsync($"api/v1/user/{currentUserId}");
        responseCurrentUser.EnsureSuccessStatusCode();
        var currentUser = await responseCurrentUser.Content.ReadFromJsonAsync<User>();

        // Prepare ViewBag for 'Go back!' button
        var responseViewedFirstProfile = await httpClient.GetAsync($"api/v1/user/{currentUser.Id}/not-seen-any-user");
        responseViewedFirstProfile.EnsureSuccessStatusCode();
        bool viewedFirstProfile = await responseViewedFirstProfile.Content.ReadFromJsonAsync<bool>();
        ViewBag.ViewedFirstProfile = viewedFirstProfile;

        var responseUsers = await httpClient.GetAsync("api/v1/user");
        responseUsers.EnsureSuccessStatusCode();
        var users = await responseUsers.Content.ReadFromJsonAsync<List<User>>();

        var allUsers = users.Where(u => u.Id != currentUser.Id).ToList();

        List<IUser> unseenUsers = new();

        foreach (var user in allUsers)
        {
            var responseIsUserSeen = await httpClient.GetAsync($"api/v1/user/{currentUser.Id}/has-seen-user/{user.Id}");
            responseIsUserSeen.EnsureSuccessStatusCode();
            bool isUserSeen = await responseIsUserSeen.Content.ReadFromJsonAsync<bool>();

            if (!isUserSeen)
            {
                unseenUsers.Add(user);
            }
        }

        IUser? randomUser = null;

        if (unseenUsers.Any())
        {
            Random rnd = new();
            randomUser = unseenUsers[rnd.Next(unseenUsers.Count)];

            var responseUserSeen =
                await httpClient.PostAsync($"api/v1/user/{currentUser.Id}/user-seen/{randomUser.Id}", null);
            responseUserSeen.EnsureSuccessStatusCode();
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

        var httpClient = _clientFactory.CreateClient("StudyBuddy.API");
        var responseCurrentUser = await httpClient.GetAsync($"api/v1/user/{currentUserId}");
        responseCurrentUser.EnsureSuccessStatusCode();

        var currentUser = await responseCurrentUser.Content.ReadFromJsonAsync<User>();

        var responsePenultimateUser = await httpClient.GetAsync($"api/v1/user/{currentUser.Id}/penultimate-seen-user");
        responsePenultimateUser.EnsureSuccessStatusCode();

        var penultimateUser = await responsePenultimateUser.Content.ReadFromJsonAsync<User>();

        TempData["HideGoBackButton"] = true;

        return View("RandomProfile", penultimateUser);
    }


    public async Task<IActionResult> UltimateProfile()
    {
        // Pass the current user's ID to the view
        UserId currentUserId = (UserId)_userSessionService.GetCurrentUserId()!;

        ViewBag.CurrentUserId = currentUserId;
        ViewBag.ShowMatchRequestMessage = false;

        var httpClient = _clientFactory.CreateClient("StudyBuddy.API");
        var responseCurrentUser = await httpClient.GetAsync($"api/v1/user/{currentUserId}");
        responseCurrentUser.EnsureSuccessStatusCode();

        var currentUser = await responseCurrentUser.Content.ReadFromJsonAsync<User>();

        var responseUltimateUser = await httpClient.GetAsync($"api/v1/user/{currentUser.Id}/ultimate-seen-user");
        responseUltimateUser.EnsureSuccessStatusCode();

        var previousUser = await responseUltimateUser.Content.ReadFromJsonAsync<User>();

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

            MatchDto matchDto = new()
            {
                currentUserId = currentUserId,
                otherUserId = otherUserId
            };

            var httpClient = _clientFactory.CreateClient("StudyBuddy.API");
            var responseMatching = await httpClient.PostAsJsonAsync("api/v1/matching/match-users", matchDto);
            responseMatching.EnsureSuccessStatusCode();

            var responseIsMatched = await httpClient.GetAsync($"api/v1/matching/is-matched/{currentUserId}/{otherUserId}");
            responseIsMatched.EnsureSuccessStatusCode();

            bool isMatched = await responseIsMatched.Content.ReadFromJsonAsync<bool>();

            if (isMatched)
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
            _logger.LogError("An error occurred while matching user {currentUser} to {otherUser}", currentUser,
                otherUser);
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
