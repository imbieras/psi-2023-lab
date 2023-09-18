using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Controllers;

public class ProfileController : Controller
{
    private readonly IUserManager _userManager; // Inject IUserManager

    public ProfileController(IUserManager userManager)
    {

        _userManager = userManager;
        _userManager.LoadUsersFromCsv("lol.csv");

    }

    public IActionResult DisplayProfiles()
    {
        try
        {
           
            List<IUser> userList = _userManager.GetAllUsers();
        //    _userManager.WriteUsersToCsv("lol.csv");

            return View(userList);
        }
        catch (Exception ex)
        {
            // Log the exception
            ViewBag.ErrorMessage = "An error occurred while retrieving user profiles. " + ex.Message;
            return View(new List<IUser>()); // Provide an empty list
        }
    }

    public IActionResult UserId(string id)
    {
        UserId parseUserId = (UserId)Guid.Parse(id);

        IUser? user = _userManager.GetUserById(parseUserId);

        if (user != null)
        {
            return View("DisplayProfiles", new List<IUser?>() { user });
        }

        ErrorViewModel errorModel = new()
        {
            ErrorMessage = "User not found."
        };
        return View("Error", errorModel);

    }

    public IActionResult CreateProfile() => View();

    [HttpPost]
    public async Task<IActionResult> SaveProfile(string name, string birthdate, string subject, IFormFile? avatar)
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

            UserTraits traits = new(
            DateTime.Parse(birthdate),
            subject,
            avatarPath
            );

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
}
