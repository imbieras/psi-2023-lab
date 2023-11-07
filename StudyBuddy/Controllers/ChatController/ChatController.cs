using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Abstractions;
using StudyBuddy.Services.UserService;
using StudyBuddy.ValueObjects;
using StudyBuddy.Hubs;
using StudyBuddy.Models;
using StudyBuddy.Services;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.Services.MatchingService;
using NuGet.Protocol.Plugins;

namespace StudyBuddy.Controllers.ChatController
{
    public class ChatController : Controller
    {
        private const string LoginPath = "~/Views/Profile/Login.cshtml";
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserService _userService;
        private readonly IUserSessionService _userSessionService;
        private readonly IMatchingService _matchingService;
        private readonly MessageService _messageService;

        public ChatController(IHubContext<ChatHub> hubContext, IUserService userService, MessageService messageService,
            IUserSessionService userSessionService, IMatchingService matchingService)
        {
            _hubContext = hubContext;
            _userService = userService;
            _messageService = messageService;
            _userSessionService = userSessionService;
            _matchingService = matchingService;

        }


        public async Task<IActionResult> ChatAsync()
        {

            UserId? currentUserId = _userSessionService.GetCurrentUserId();

            if (!Guid.TryParse(currentUserId.ToString(), out Guid userIdGuid))
            {
                return View(LoginPath);
            }

            UserId parseUserId = UserId.From(userIdGuid);

            IUser? currentUser = await _userService.GetUserByIdAsync((UserId)currentUserId);

            if (currentUser == null)
            {
                return View(LoginPath);
            }

            //Show list of matched users
            List<Match> matches = (List<Match>)await _matchingService.GetMatchHistoryAsync((UserId)currentUserId);

            List<IUser> userList = new List<IUser>();

            foreach (var match in matches)
            {
                if (match.User1Id == currentUser.Id)
                {
                    userList.Add(await _userService.GetUserByIdAsync(match.User2Id));
                }
                else
                {

                    userList.Add(await _userService.GetUserByIdAsync(match.User1Id));
                }

            }

            // Pass both the current user and the other user to the view
            var viewModel = new ChatViewModel
            {
                CurrentUser = currentUser,
                Matches = userList
            };


            return View(viewModel);
        }

        public async Task<IActionResult> ChatWithUserAsync(string otherUserId)
        {
            // Retrieve the current user
            UserId? currentUserId = _userSessionService.GetCurrentUserId();

            if (!Guid.TryParse(currentUserId.ToString(), out Guid userIdGuid))
            {
                return View(LoginPath);
            }

            UserId parseUserId = UserId.From(userIdGuid);
            IUser? currentUser = await _userService.GetUserByIdAsync(parseUserId);

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
            IUser? otherUser = await _userService.GetUserByIdAsync(parseOtherUserId);

            if (otherUser == null)
            {
                return View(LoginPath); //change later

            }

            byte[] bytes1 = userIdGuid.ToByteArray();
            byte[] bytes2 = otherUserIdGuid.ToByteArray();

            for (int i = 0; i < bytes1.Length; i++)
            {
                bytes1[i] = (byte)(bytes1[i] ^ bytes2[i]);
            }

            Guid groupName = new Guid(bytes1);

            // Add both users to the same SignalR group
            _hubContext.Groups.AddToGroupAsync(currentUser.Id.ToString(), groupName.ToString());
            _hubContext.Groups.AddToGroupAsync(otherUser.Id.ToString(), groupName.ToString());


            //Show list of matched users
            List<Match> matches = (List<Match>)await _matchingService.GetMatchHistoryAsync(currentUser.Id);

            List<IUser> userList = new List<IUser>();

            foreach (var match in matches)
            {
                if (match.User1Id == currentUser.Id)
                {
                    userList.Add(await _userService.GetUserByIdAsync(match.User2Id));
                }
                else
                {

                    userList.Add(await _userService.GetUserByIdAsync(match.User1Id));
                }

            }
            List<Models.Message> messageList = _messageService.GetMessages(groupName.ToString());

            // Pass both the current user and the other user to the view
            var viewModel = new ChatViewModel
            {
                CurrentUser = currentUser,
                OtherUser = otherUser,
                Matches = userList,
                messages = messageList,
                GroupName = groupName
            };

            return View(viewModel);
        }







    }
}
