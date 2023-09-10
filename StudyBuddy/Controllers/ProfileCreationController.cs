using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;

namespace StudyBuddy.Controllers;

public class ProfileCreationController : Controller
{
    private readonly IUserManager _userManager;

    public ProfileCreationController(IUserManager userManager)
    {
        _userManager = userManager;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProcessNewProfile(string name, string birthdate, string subject, IFormFile? avatar)
    {
        const UserFlags flags = UserFlags.Registered;

        if (avatar is { Length: > 0 })
        {
            try
            {
                string uniqueFileName = Guid.NewGuid() + "_" + avatar.FileName;
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\avatars", uniqueFileName);

                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(fileStream);
                }

                var userId = _userManager.RegisterUser(name, flags, DateTime.Parse(birthdate), subject, uniqueFileName);

                IUser? user = _userManager.GetUserById(userId);

                return View(user);
            }
            catch (Exception ex)
            {
                return BadRequest("Error uploading file: " + ex.Message);
            }
        }

        if (!DateTime.TryParse(birthdate, out DateTime parsedBirthdate))
        {
            return BadRequest("Invalid birthdate format");
        }

        var userIdWithoutAvatar = _userManager.RegisterUser(name, flags, parsedBirthdate, subject, string.Empty);

        IUser? userWithoutAvatar = _userManager.GetUserById(userIdWithoutAvatar);

        return View(userWithoutAvatar);
    }
}
