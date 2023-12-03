using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Attributes;
using StudyBuddy.Hubs;
using StudyBuddy.Services.UserSessionService;
using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.Utilities;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.Controllers.ChatController;

[CustomAuthorize]
public class ChatController : Controller
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IUserSessionService _userSessionService;
    private readonly IHttpClientFactory _clientFactory;

    public ChatController(IHubContext<ChatHub> hubContext, IUserSessionService userSessionService,
        IHttpClientFactory clientFactory)
    {
        _hubContext = hubContext;
        _userSessionService = userSessionService;
        _clientFactory = clientFactory;
    }

    public async Task<IActionResult> ChatAsync()
    {
        UserId currentUserId = (UserId)_userSessionService.GetCurrentUserId()!;

        HttpClient? httpClient = _clientFactory.CreateClient("StudyBuddy.API");
        HttpResponseMessage? responseCurrentUser = await httpClient.GetAsync($"api/v1/user/{currentUserId}");
        responseCurrentUser.EnsureSuccessStatusCode();

        User? currentUser = await responseCurrentUser.Content.ReadFromJsonAsync<User>();

        //Show list of matched users
        HttpResponseMessage? responseMatchHistory =
            await httpClient.GetAsync($"api/v1/matching/match-history/{currentUserId}");
        responseMatchHistory.EnsureSuccessStatusCode();

        List<Match>? matches = await responseMatchHistory.Content.ReadFromJsonAsync<List<Match>>();

        List<IUser?> userList = new();

        foreach (Match? match in matches)
        {
            if (match.User1Id == currentUserId)
            {
                HttpResponseMessage? responseMatchUser2 = await httpClient.GetAsync($"api/v1/user/{match.User2Id}");
                responseMatchUser2.EnsureSuccessStatusCode();
                userList.Add(await responseMatchUser2.Content.ReadFromJsonAsync<User>());
            }
            else
            {
                HttpResponseMessage? responseMatchUser1 = await httpClient.GetAsync($"api/v1/user/{match.User1Id}");
                responseMatchUser1.EnsureSuccessStatusCode();
                userList.Add(await responseMatchUser1.Content.ReadFromJsonAsync<User>());
            }
        }

        // Pass both the current user and the other user to the view
        ChatViewModel viewModel = new() { CurrentUser = currentUser, Matches = userList };


        return View(viewModel);
    }

    public async Task<IActionResult> ChatWithUserAsync(string otherUserId)
    {
        UserId currentUserId = (UserId)_userSessionService.GetCurrentUserId()!;

        HttpClient? httpClient = _clientFactory.CreateClient("StudyBuddy.API");
        HttpResponseMessage? responseCurrentUser = await httpClient.GetAsync($"api/v1/user/{currentUserId}");
        responseCurrentUser.EnsureSuccessStatusCode();

        User? currentUser = await responseCurrentUser.Content.ReadFromJsonAsync<User>();

        // Retrieve the other user
        if (!Guid.TryParse(otherUserId, out Guid otherUserIdGuid))
        {
            ViewBag.ErrorMessage = "Couldn't parse receivers ID.";
            return View("ChatError");
        }

        UserId parseOtherUserId = UserId.From(otherUserIdGuid);

        HttpResponseMessage? responseOtherUser = await httpClient.GetAsync($"api/v1/user/{parseOtherUserId}");
        responseOtherUser.EnsureSuccessStatusCode();
        User? otherUser = await responseOtherUser.Content.ReadFromJsonAsync<User>();

        HttpResponseMessage? responseMatchingIsMatched =
            await httpClient.GetAsync($"api/v1/matching/is-matched/{currentUserId}/{parseOtherUserId}");
        responseMatchingIsMatched.EnsureSuccessStatusCode();

        bool isMatched = await responseMatchingIsMatched.Content.ReadFromJsonAsync<bool>();

        if (otherUser == null || !isMatched)
        {
            ViewBag.ErrorMessage = "Trying to chat with unmatched user.";
            return View("ChatError");
        }

        // Generate a unique group name for the two users
        Guid currentUserIdGuid = Guid.Parse(currentUserId.ToString() ?? string.Empty);

        Guid groupName = ConversationIdHelper.GetGroupId(currentUserIdGuid, otherUserIdGuid);

        // Add both users to the same SignalR group
        await _hubContext.Groups.AddToGroupAsync(currentUser.Id.ToString() ?? string.Empty, groupName.ToString());
        await _hubContext.Groups.AddToGroupAsync(otherUser.Id.ToString() ?? string.Empty, groupName.ToString());


        //Show list of matched users
        HttpResponseMessage? responseMatchHistory =
            await httpClient.GetAsync($"api/v1/matching/match-history/{currentUser.Id}");
        responseMatchHistory.EnsureSuccessStatusCode();

        List<Match>? matches = await responseMatchHistory.Content.ReadFromJsonAsync<List<Match>>();

        List<IUser?> matchList = new();

        foreach (Match? match in matches)
        {
            if (match.User1Id == currentUser.Id)
            {
                HttpResponseMessage? responseMatchUser2 = await httpClient.GetAsync($"api/v1/user/{match.User2Id}");
                responseMatchUser2.EnsureSuccessStatusCode();
                matchList.Add(await responseMatchUser2.Content.ReadFromJsonAsync<User>());
            }
            else
            {
                HttpResponseMessage? responseMatchUser1 = await httpClient.GetAsync($"api/v1/user/{match.User1Id}");
                responseMatchUser1.EnsureSuccessStatusCode();
                matchList.Add(await responseMatchUser1.Content.ReadFromJsonAsync<User>());
            }
        }

        // Retrieve the messages for the conversation
        HttpResponseMessage? responseConversationMessages =
            await httpClient.GetAsync($"api/v1/chat/conversations/{groupName}/messages");
        responseConversationMessages.EnsureSuccessStatusCode();

        List<ChatMessage>? messageList =
            await responseConversationMessages.Content.ReadFromJsonAsync<List<ChatMessage>>();

        TimeZoneInfo userTimeZone = TimeZoneInfo.Local;

        // Convert UTC timestamps to the user's local time zone
        foreach (ChatMessage message in messageList)
        {
            message.Timestamp = TimeZoneInfo.ConvertTimeFromUtc(message.Timestamp, userTimeZone);
        }

        ChatViewModel viewModel = new()
        {
            CurrentUser = currentUser,
            OtherUser = otherUser,
            Matches = matchList,
            Messages = messageList,
            GroupName = groupName
        };

        return View(viewModel);
    }
}
