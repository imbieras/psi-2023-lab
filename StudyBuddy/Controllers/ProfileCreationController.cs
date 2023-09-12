using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Controllers;

public class ProfileCreationController : Controller
{
    private readonly IUserManager _userManager;

    public ProfileCreationController(IUserManager userManager) => _userManager = userManager;

    // GET
    public IActionResult Index() => View();

    [HttpPost]
    public async Task<IActionResult> DisplayNewProfile(string name, string birthdate, string subject, IFormFile? avatar)
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

            UserId userId = _userManager.RegisterUser(name, flags, traits);

            IUser? user = _userManager.GetUserById(userId);

            return View(user);
        }
        catch (Exception ex)
        {
            return BadRequest("Error uploading file: " + ex.Message);
        }
    }
}
