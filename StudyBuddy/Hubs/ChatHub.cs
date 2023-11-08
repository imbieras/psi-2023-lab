using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Abstractions;
using StudyBuddy.Data.Repositories.ChatRepository;
using StudyBuddy.Models;
using StudyBuddy.Services;
using StudyBuddy.Services.UserService;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Hubs;

public class ChatHub : Hub
{

    private readonly IUserService _userService;
    private readonly ChatRepository _chatRepository;
    private readonly ChatService _chatService;


    public ChatHub(IUserService userService, ChatRepository chatRepository)
    {
        _userService = userService;
        _chatRepository = chatRepository;
    }


    public override async Task OnConnectedAsync()
    {
        HttpContext httpContext = Context.GetHttpContext();
        string receiver = httpContext.Request.Query["receiver"];
        string sender = httpContext.Request.Query["sender"];

        UserId.TryParse(receiver, out UserId receiverId);
        UserId.TryParse(sender, out UserId senderId);


        if (!string.IsNullOrEmpty(sender) && !string.IsNullOrEmpty(receiver))
        {
            Guid.TryParse(receiver, out Guid receiverGuid);
            Guid.TryParse(sender, out Guid senderGuid);

            // Sort user IDs to ensure consistent group names regardless of user roles
            var userIds = new List<string> { sender, receiver };
            userIds.Sort();



            byte[] bytes1 = senderGuid.ToByteArray();
            byte[] bytes2 = receiverGuid.ToByteArray();

            for (int i = 0; i < bytes1.Length; i++)
            {
                bytes1[i] = (byte)(bytes1[i] ^ bytes2[i]);
            }

            Guid groupName = new Guid(bytes1);
            Conversation conversation = new Conversation(senderId, receiverId);


            Console.WriteLine("Context.ConnectionId: " + Context.ConnectionId + "\nSENDER:" + sender + "\nRECEIVER " + receiver + "\nGROUP_NAME: " + groupName);
            // Add the current connection to the conversation group
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName.ToString());
        }



        await base.OnConnectedAsync();
    }

    public async Task SendMessageToGroup(Guid groupName, string message)
    {
        HttpContext httpContext = Context.GetHttpContext();

        string receiver = httpContext.Request.Query["receiver"];
        string sender = httpContext.Request.Query["sender"];


        UserId.TryParse(httpContext.Request.Query["sender"], out UserId senderId);
        UserId.TryParse(httpContext.Request.Query["receiver"], out UserId receiverId);

        ChatMessage chatMessage = new ChatMessage(message, DateTime.UtcNow.AddHours(2), senderId, groupName);

        Console.WriteLine($"SENDING MESSAGE TO:{sender} TEXT:{message} GROUP NAME {groupName.ToString()}");



        await _chatRepository.AddMessageAsync(chatMessage);


        // Broadcast the message to the conversation group
        await Clients.Group(groupName.ToString()).SendAsync("ReceiveMessage", sender, message);
    }


    public async Task ReceiveMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }


}
