using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Data.Repositories.ChatRepository;
using StudyBuddy.Models;
using StudyBuddy.Services.ChatService;
using StudyBuddy.Utilities;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Hubs;

public class ChatHub : Hub
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService) => _chatService = chatService;

    public override async Task OnConnectedAsync()
    {
        HttpContext httpContext = Context.GetHttpContext();
        string receiver = httpContext.Request.Query["receiver"];
        string sender = httpContext.Request.Query["sender"];

        if (!string.IsNullOrEmpty(sender) && !string.IsNullOrEmpty(receiver))
        {
            Guid.TryParse(receiver, out Guid receiverGuid);
            Guid.TryParse(sender, out Guid senderGuid);

            Guid groupName = ConversationIdHelper.GetGroupId(senderGuid, receiverGuid);

            Console.WriteLine("Context.ConnectionId: " + Context.ConnectionId + "\nSENDER:" + sender + "\nRECEIVER " +
                              receiver + "\nGROUP_NAME: " + groupName);
            // Add the current connection to the conversation group
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName.ToString());
        }


        await base.OnConnectedAsync();
    }

    public async Task SendMessageToGroup(Guid groupName, string message)
    {
        HttpContext httpContext = Context.GetHttpContext();

        string sender = httpContext.Request.Query["sender"];

        UserId.TryParse(sender, out UserId senderId);

        ChatMessage chatMessage = new(message, DateTime.UtcNow.AddHours(2), senderId, groupName);

        Console.WriteLine($"SENDING MESSAGE TO:{sender} TEXT:{message} GROUP NAME {groupName.ToString()}");

        await _chatService.AddMessageAsync(chatMessage);

        // Broadcast the message to the conversation group
        await Clients.Group(groupName.ToString()).SendAsync("ReceiveMessage", sender, message);
    }


    public async Task ReceiveMessage(string user, string message) =>
        await Clients.All.SendAsync("ReceiveMessage", user, message);
}
