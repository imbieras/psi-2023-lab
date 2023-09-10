using Microsoft.AspNetCore.Mvc;

namespace StudyBuddy.Controllers
{
    public class ExploreProfilesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
