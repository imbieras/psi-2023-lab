using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Abstractions;
using StudyBuddy.Models;
using StudyBuddy.Services;
using StudyBuddy.Services.UserService;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Hubs;

public class ChatHub : Hub
{

    private readonly IUserService _userService;
    private readonly MessageService _messageService;


    public ChatHub(IUserService userService, MessageService messageService)
    {
        _userService = userService;
        _messageService = messageService;
    }


    public override async Task OnConnectedAsync()
    {
        HttpContext httpContext = Context.GetHttpContext();
        string receiver = httpContext.Request.Query["receiver"];
        string sender = httpContext.Request.Query["sender"];

        if (!string.IsNullOrEmpty(sender) && !string.IsNullOrEmpty(receiver))
        {
            // Sort user IDs to ensure consistent group names regardless of user roles
            var userIds = new List<string> { sender, receiver };
            userIds.Sort();
            string groupName = $"{userIds[0]}-{userIds[1]}";


            Console.WriteLine("Context.ConnectionId: " + Context.ConnectionId + "\nSENDER:" + sender + "\nRECEIVER " + receiver + "\nGROUP_NAME: " + groupName);
            // Add the current connection to the conversation group
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }



        await base.OnConnectedAsync();
    }

    public Task SendMessageToGroup(string groupName, string message)
    {
        HttpContext httpContext = Context.GetHttpContext();

        string receiver = httpContext.Request.Query["receiver"];
        string sender = httpContext.Request.Query["sender"];


        UserId.TryParse(httpContext.Request.Query["sender"], out UserId senderId);

        if (Guid.TryParse(groupName, out Guid parsedGroupName))
            Console.WriteLine("PARRSSEEED: " + parsedGroupName);

        //ChatMessage chatMessage = new ChatMessage(message, DateTime.Now, senderId, groupName);
        Message sentMessage = new Message();
        sentMessage.SenderId = senderId;
        sentMessage.Text = message;
        sentMessage.Time = DateTime.UtcNow;

        Console.WriteLine($"SENDING MESSAGE TO:{sender} TEXT:{message} GROUP NAME {groupName}");

        _messageService.AddMessage(groupName, sentMessage);

        // Broadcast the message to the conversation group
        return Clients.Group(groupName).SendAsync("ReceiveMessage", sender, message);
    }



    public async Task SendMessage(string sender, string message)
    {
        // Broadcast the message to the sender's group
        string groupForSender = $"user-{sender}";
        await Clients.Group(groupForSender).SendAsync("ReceiveMessage", sender, message);
    }


    public async Task ReceiveMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }


}
