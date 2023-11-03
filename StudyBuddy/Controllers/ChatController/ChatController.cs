using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Abstractions;
using StudyBuddy.Managers.MatchingManager;
using StudyBuddy.Managers.UserManager;
using StudyBuddy.Services.UserService;
using StudyBuddy.ValueObjects;
using StudyBuddy.Hubs;
using StudyBuddy.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using StudyBuddy.Services;

namespace StudyBuddy.Controllers.ChatController
{
    public class ChatController : Controller
    {
        private const string LoginPath = "~/Views/Profile/Login.cshtml";
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IMatchingManager _matchingManager;
        private readonly IUserManager _userManager;
        private readonly IUserService _userService;
        private readonly MessageService _messageService;

        public ChatController(IHubContext<ChatHub> hubContext, IUserManager userManager, IMatchingManager matchingManager, IUserService userService, MessageService messageService)
        {
            _hubContext = hubContext;
            _userManager = userManager;
            _matchingManager = matchingManager;
            _userService = userService;
            _messageService = messageService;

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

            //Show list of matched users
            List<IUser> matches = new List<IUser>();
            List<IUser> userList = _userManager.GetAllUsers();

            foreach (var user in userList)
            {
                if (_matchingManager.IsMatched(currentUser.Id, user.Id))
                {
                    matches.Add(user);
                }

            }

            // Pass both the current user and the other user to the view
            var viewModel = new ChatViewModel
            {
                CurrentUser = currentUser,
                Matches = matches
            };


            return View(viewModel);
        }

        public IActionResult ChatWithUser(string otherUserId)
        {
            // Retrieve the current user
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

            // Retrieve the other user
            if (!Guid.TryParse(otherUserId, out Guid otherUserIdGuid))
            {
                return View(LoginPath); //change later
            }

            UserId parseOtherUserId = UserId.From(otherUserIdGuid);
            IUser? otherUser = _userManager.GetUserById(parseOtherUserId);

            if (otherUser == null)
            {
                return View(LoginPath); //change later

            }

            // Sort user IDs to ensure consistent group names regardless of user roles
            var userIds = new List<UserId> { currentUser.Id, otherUser.Id };
            userIds.Sort();

            // Generate a unique group name based on user IDs
            string groupName = $"{userIds[0]}-{userIds[1]}";

            // Add both users to the same SignalR group
            _hubContext.Groups.AddToGroupAsync(currentUser.Id.ToString(), groupName);
            _hubContext.Groups.AddToGroupAsync(otherUser.Id.ToString(), groupName);


            //Show list of matched users
            List<IUser> matches = new List<IUser>();
            List<IUser> userList = _userManager.GetAllUsers();
            List<string> messageList = _messageService.GetMessages(groupName);

            foreach (var user in userList)
            {
                if (_matchingManager.IsMatched(currentUser.Id, user.Id))
                {
                    matches.Add(user);
                }

            }

            // Pass both the current user and the other user to the view
            var viewModel = new ChatViewModel
            {
                CurrentUser = currentUser,
                OtherUser = otherUser,
                Matches = matches,
                messages = messageList
            };

            return View(viewModel);
        }







    }
}
