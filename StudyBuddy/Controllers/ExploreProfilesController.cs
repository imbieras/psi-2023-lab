using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Managers;
using StudyBuddy.Models;

namespace StudyBuddy.Controllers
{
    public class ExploreProfilesController : Controller
    {

        private readonly IUserManager _userManager; // Inject IUserManager

        public ExploreProfilesController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            try
            {
                List<IUser> userList = _userManager.GetAllUsers();
                return View(userList);
            }
            catch (Exception ex)
            {
                // Log the exception and handle it gracefully
                ViewBag.ErrorMessage = "An error occurred while retrieving user profiles.";
                return View(new List<User>()); // Provide an empty list or handle the error as needed
            }
        }

    }
}
