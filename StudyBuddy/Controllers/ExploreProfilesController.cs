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
            var userList = _userManager.GetUserList();

            // Pass the list of users to the view
            return View(userList);
        }

    }
}
