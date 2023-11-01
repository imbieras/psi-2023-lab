using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Abstractions;
using StudyBuddy.Managers.MatchingManager;
using StudyBuddy.Managers.UserManager;
using StudyBuddy.Services.UserService;
using StudyBuddy.ValueObjects;
using StudyBuddy.Hubs;

namespace StudyBuddy.Controllers.ChatController
{
    public class ChatController : Controller
    {
        private const string LoginPath = "~/Views/Profile/Login.cshtml";
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMatchingManager _matchingManager;
        private readonly IUserManager _userManager;
        private readonly IUserService _userService;

        public ChatController(IHubContext<ChatHub> hubContext, IUserManager userManager, IMatchingManager matchingManager, IUserService userService)
        {
            _hubContext = hubContext;
            _userManager = userManager;
            _matchingManager = matchingManager;
            _userService = userService;

        }


        public IActionResult Chat()
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

            return View(currentUser);
        }
    }
}
