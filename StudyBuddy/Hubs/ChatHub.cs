using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Data.Repositories.ChatRepository;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Hubs;

public class ChatHub : Hub
{
    private readonly IChatRepository _chatRepository;

    public ChatHub(IChatRepository chatRepository) => _chatRepository = chatRepository;

    public override async Task OnConnectedAsync()
    {
        HttpContext httpContext = Context.GetHttpContext();
        string receiver = httpContext.Request.Query["receiver"];
        string sender = httpContext.Request.Query["sender"];

        if (!string.IsNullOrEmpty(sender) && !string.IsNullOrEmpty(receiver))
        {
            Guid.TryParse(receiver, out Guid receiverGuid);
            Guid.TryParse(sender, out Guid senderGuid);

            byte[] bytes1 = senderGuid.ToByteArray();
            byte[] bytes2 = receiverGuid.ToByteArray();

            for (int i = 0; i < bytes1.Length; i++)
            {
                bytes1[i] = (byte)(bytes1[i] ^ bytes2[i]);
            }

            Guid groupName = new(bytes1);

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

        ChatMessage chatMessage = new(message, DateTime.UtcNow, senderId, groupName);

        await _chatRepository.AddMessageAsync(chatMessage);

        // Broadcast the message to the conversation group
        await Clients.Group(groupName.ToString()).SendAsync("ReceiveMessage", sender, message);
    }


    public async Task ReceiveMessage(string user, string message) =>
        await Clients.All.SendAsync("ReceiveMessage", user, message);
}
