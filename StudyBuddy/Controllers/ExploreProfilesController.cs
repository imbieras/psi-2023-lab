using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyBuddy.Abstractions;
using StudyBuddy.Managers;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

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
                // Log the exception
                ViewBag.ErrorMessage = "An error occurred while retrieving user profiles.";
                return View(new List<IUser>()); // Provide an empty list
            }
        }

        public IActionResult Profile(string id)
        {

            UserId parseUserId= (UserId)Guid.Parse(id);

            IUser? user = _userManager.GetUserById(parseUserId);

            if (user != null)
            {
                return View("Index", new List<IUser?>() {user});
            }
            else
            {
                ErrorViewModel errorModel = new ErrorViewModel
                {
                    ErrorMessage = "User not found.",
                };
                return View("Error", errorModel);
            }

        }
    }
}
