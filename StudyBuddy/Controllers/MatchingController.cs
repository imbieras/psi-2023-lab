﻿using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Attributes;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.Shared.Abstractions;
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

        var currentUser = await responseCurrentUser.Content.ReadFromJsonAsync<IUser>();

        var responseUltimateUser = await httpClient.GetAsync($"api/v1/user/{currentUser.Id}/ultimate-seen-user");
        responseUltimateUser.EnsureSuccessStatusCode();

        var currentRandomUser = await responseUltimateUser.Content.ReadFromJsonAsync<IUser>();

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
        var currentUser = await responseCurrentUser.Content.ReadFromJsonAsync<IUser>();

        // Prepare ViewBag for 'Go back!' button
        var responseViewedFirstProfile = await httpClient.GetAsync($"api/v1/user/{currentUser.Id}/not-seen-any-user");
        responseViewedFirstProfile.EnsureSuccessStatusCode();
        bool viewedFirstProfile = await responseViewedFirstProfile.Content.ReadFromJsonAsync<bool>();
        ViewBag.ViewedFirstProfile = viewedFirstProfile;

        var responseUnseenUsers = await httpClient.GetAsync($"api/v1/user");
        responseUnseenUsers.EnsureSuccessStatusCode();
        var allUsers = await responseUnseenUsers.Content.ReadFromJsonAsync<List<IUser>>();

        var responseSeenUsers = await httpClient.GetAsync($"api/v1/user/{currentUser.Id}/has-seen-user");
        responseSeenUsers.EnsureSuccessStatusCode();
        var seenUsers = await responseSeenUsers.Content.ReadFromJsonAsync<List<UserId>>();

        var unseenUsers = allUsers.Where(u => !seenUsers.Contains(u.Id) && u.Id != currentUser.Id).ToList();

        IUser? randomUser = null;

        if (unseenUsers.Any())
        {
            Random rnd = new();
            randomUser = unseenUsers[rnd.Next(unseenUsers.Count)];

            var responseUserSeen =
                await httpClient.PostAsync($"api/v1/user/{currentUser.Id}/has-seen-user/{randomUser.Id}", null);
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

        var currentUser = await responseCurrentUser.Content.ReadFromJsonAsync<IUser>();

        var responsePenultimateUser = await httpClient.GetAsync($"api/v1/user/{currentUser.Id}/penultimate-seen-user");
        responsePenultimateUser.EnsureSuccessStatusCode();

        var penultimateUser = await responsePenultimateUser.Content.ReadFromJsonAsync<IUser>();

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

        var currentUser = await responseCurrentUser.Content.ReadFromJsonAsync<IUser>();

        var responseUltimateUser = await httpClient.GetAsync($"api/v1/user/{currentUser.Id}/ultimate-seen-user");
        responseUltimateUser.EnsureSuccessStatusCode();

        var previousUser = await responseUltimateUser.Content.ReadFromJsonAsync<IUser>();

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

            var httpClient = _clientFactory.CreateClient("StudyBuddy.API");
            var responseMatching = await httpClient.GetAsync($"api/v1/matching/match/{currentUserId}/{otherUserId}");
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
