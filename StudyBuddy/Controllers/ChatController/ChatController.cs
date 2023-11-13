using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Abstractions;
using StudyBuddy.Services.UserService;
using StudyBuddy.ValueObjects;
using StudyBuddy.Hubs;
using StudyBuddy.Models;
using StudyBuddy.Services.ChatService;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.Services.MatchingService;

namespace StudyBuddy.Controllers.ChatController;

public class ChatController : Controller
{
    private const string LoginPath = "~/Views/Profile/Login.cshtml";
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IUserService _userService;
    private readonly IUserSessionService _userSessionService;
    private readonly IMatchingService _matchingService;
    private readonly IChatService _chatService;

    public ChatController(IHubContext<ChatHub> hubContext, IUserService userService,
        IUserSessionService userSessionService, IMatchingService matchingService, IChatService chatService)
    {
        _hubContext = hubContext;
        _userService = userService;
        _userSessionService = userSessionService;
        _matchingService = matchingService;
        _chatService = chatService;
    }


    public async Task<IActionResult> ChatAsync()
    {
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

        //Show list of matched users
        List<Match> matches = (List<Match>)await _matchingService.GetMatchHistoryAsync(currentUser.Id);

        List<IUser?> userList = new();

        foreach (Match? match in matches)
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
        ChatViewModel? viewModel = new ChatViewModel { CurrentUser = currentUser, Matches = userList };


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
            ViewBag.ErrorMessage = "Couldn't parse receivers ID.";
            return View("ChatError");
        }

        UserId parseOtherUserId = UserId.From(otherUserIdGuid);
        IUser? otherUser = await _userService.GetUserByIdAsync(parseOtherUserId);

        if (otherUser == null || !await _matchingService.IsMatchedAsync(currentUser.Id, otherUser.Id))
        {
            ViewBag.ErrorMessage = "Trying to chat with unmatched user.";
            return View("ChatError");
        }

        byte[] bytes1 = userIdGuid.ToByteArray();
        byte[] bytes2 = otherUserIdGuid.ToByteArray();

        for (int i = 0; i < bytes1.Length; i++)
        {
            bytes1[i] = (byte)(bytes1[i] ^ bytes2[i]);
        }

        Guid groupName = new(bytes1);

        // Add both users to the same SignalR group
        await _hubContext.Groups.AddToGroupAsync(currentUser.Id.ToString() ?? string.Empty, groupName.ToString());
        await _hubContext.Groups.AddToGroupAsync(otherUser.Id.ToString() ?? string.Empty, groupName.ToString());


        //Show list of matched users
        List<Match> matches = (List<Match>)await _matchingService.GetMatchHistoryAsync(currentUser.Id);

        List<IUser?> matchList = new();

        foreach (Match? match in matches)
        {
            if (match.User1Id == currentUser.Id)
            {
                matchList.Add(await _userService.GetUserByIdAsync(match.User2Id));
            }
            else
            {
                matchList.Add(await _userService.GetUserByIdAsync(match.User1Id));
            }
        }

        List<ChatMessage> messageList = (List<ChatMessage>)await _chatService.GetMessagesByConversationAsync(groupName);

        TimeZoneInfo userTimeZone = TimeZoneInfo.Local;

        // Convert UTC timestamps to the user's local time zone
        foreach (var message in messageList)
        {
            message.Timestamp = TimeZoneInfo.ConvertTimeFromUtc(message.Timestamp, userTimeZone);
        }

        ChatViewModel viewModel = new ChatViewModel
        {
            CurrentUser = currentUser,
            OtherUser = otherUser,
            Matches = matchList,
            messages = messageList,
            GroupName = groupName
        };

        return View(viewModel);
    }
}
