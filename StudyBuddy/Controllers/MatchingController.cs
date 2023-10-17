﻿using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Managers.MatchingManager;
using StudyBuddy.Managers.UserManager;
using StudyBuddy.Services.UserService;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Controllers;

public class MatchingController : Controller
{
    private const string LoginPath = "~/Views/Profile/Login.cshtml";
    private readonly IMatchingManager _matchingManager;
    private readonly IUserManager _userManager;
    private readonly IUserService _userService;

    public MatchingController(IUserManager userManager, IMatchingManager matchingManager, IUserService userService)
    {
        _userManager = userManager;
        _matchingManager = matchingManager;
        _userService = userService;
    }

    public IActionResult CurrentRandomUserProfile()
    {
        UserId? currentUserId = _userService.GetCurrentUserId();
        if (!Guid.TryParse(currentUserId.ToString(), out Guid userIdGuid))
        {
            return View(LoginPath);
        }

        UserId parseUserId = UserId.From(userIdGuid);

        IUser? currentUser = _userManager.GetUserById(parseUserId);

        if (currentUser == null)
        {
            return View(LoginPath);
        }

        IUser? currentRandomUser = _userManager.GetCurrentRandomUser(currentUser);
        ViewBag.ShowMatchRequestMessage = true;

        return View("RandomProfile", currentRandomUser);
    }

    public IActionResult RandomProfile()
    {
        // Pass the current user's ID to the view
        UserId? currentUserId = _userService.GetCurrentUserId();

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

        IUser? currentUser = _userManager.GetUserById(parseUserId);

        ViewBag.ViewedFirstProfile =
            currentUser != null && _userManager.IsUsedIndexesEmpty(currentUser); // For 'Go back!' button

        if (currentUser == null)
        {
            return View(LoginPath);
        }

        IUser? randomUser = null;
        int counter = 0;

        // Keep generating random users until an unmatched and unrequested user is found
        while (randomUser == null || (_matchingManager.IsMatched(currentUser.Id, randomUser.Id) &&
                                      _matchingManager.IsRequestedMatch(currentUser.Id, randomUser.Id)))
        {
            randomUser = _userManager.GetRandomUser(currentUser);

            counter++;
            if (counter > _userManager.GetAllUsers().Count)
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

    public IActionResult PreviousProfile()
    {
        // Pass the current user's ID to the view
        UserId? currentUserId = _userService.GetCurrentUserId();

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

        IUser? currentUser = _userManager.GetUserById(parseUserId);

        if (currentUser == null)
        {
            return View(LoginPath);
        }

        IUser? previousUser = _userManager.GetPreviousRandomProfile(currentUser);

        TempData["HideGoBackButton"] = true;

        return View("RandomProfile", previousUser);
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
                ViewBag.ShowMatchRequestMessage = true;
            }

            if (_matchingManager.IsMatched(currentUserId, otherUserId))
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