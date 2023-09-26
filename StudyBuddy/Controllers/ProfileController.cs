using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Managers;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;
using Markdig;

namespace StudyBuddy.Controllers;

public class ProfileController : Controller
{
    private readonly IUserManager _userManager; // Inject IUserManager
    private readonly FileManager _fileManager;

    public ProfileController(IUserManager userManager, FileManager filemanager)
    {
        _userManager = userManager;
        _fileManager = filemanager;

    }

    public IActionResult DisplayProfiles()
    {
        try
        {

            List<IUser> userList = _userManager.GetAllUsers();

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

        ErrorViewModel errorModel = new()
        {
            ErrorMessage = "User not found or invalid ID."
        };
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
            ErrorViewModel errorModel = new ErrorViewModel { ErrorMessage = "Error uploading file: " + ex.Message, };
            return View("Error", errorModel);
        }
    }



    public IActionResult RandomProfile()
    {
        List<IUser> userList = _userManager.GetAllUsers();

        // Check if there are any users to display
        if (userList.Count == 0)
        {
            ViewBag.ErrorMessage = "No user profiles available.";
            return View("Error");
        }

        // Shuffle the list of users
        Random random = new Random();
        userList = userList.OrderBy(x => random.Next()).ToList();

        // Get the first user (randomly shuffled)
        IUser randomUser = userList[0];

        // Remove the displayed user from the list
        userList.RemoveAt(0);

        return View("RandomProfile", randomUser);
    }


}
