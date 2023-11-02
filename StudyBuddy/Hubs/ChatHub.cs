using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Abstractions;
using StudyBuddy.Managers.UserManager;
using StudyBuddy.Services.UserService;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Hubs;

public class ChatHub : Hub
{

    private readonly IUserManager _userManager;
    private readonly IUserService _userService;
    public ChatHub(IUserManager userManager, IUserService userService)
    {
        _userManager = userManager;
        _userService = userService;
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

            Console.WriteLine("\nAdding user to group: " + groupName);

            // Add the current connection to the conversation group
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        Console.WriteLine("Context.ConnectionId: " + Context.ConnectionId + "\nSENDER:" + sender + "\nRECEIVER " + receiver);

        await base.OnConnectedAsync();
    }

    public Task SendMessageToGroup(string groupName, string message)
    {
        HttpContext httpContext = Context.GetHttpContext();

        UserId.TryParse(httpContext.Request.Query["sender"], out UserId senderId);


        IUser sender = _userManager.GetUserById(senderId);


        // Broadcast the message to the conversation group
        return Clients.Group(groupName).SendAsync("ReceiveMessage", sender.Id, message);
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
