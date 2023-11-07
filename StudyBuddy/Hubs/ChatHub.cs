using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Abstractions;
using StudyBuddy.Data.Repositories.UserRepository;
using StudyBuddy.Models;
using StudyBuddy.Services;
using StudyBuddy.Services.UserService;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Hubs;

public class ChatHub : Hub
{

    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly MessageService _messageService;


    public ChatHub(IUserRepository userRepository, IUserService userService, MessageService messageService)
    {
        _userRepository = userRepository;
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
            string sortedGroupName = $"{userIds[0]}-{userIds[1]}";

            Guid groupName;

            if (Guid.TryParse(sortedGroupName, out groupName))
            {
                Console.WriteLine("\nAdding user to group: " + groupName);
            }
            else
            {
                
            }

           

            // Add the current connection to the conversation group
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName.ToString());
        }

        Console.WriteLine("Context.ConnectionId: " + Context.ConnectionId + "\nSENDER:" + sender + "\nRECEIVER " + receiver);

        await base.OnConnectedAsync();
    }

    public Task SendMessageToGroup(Guid groupName, string message)
    {
        HttpContext httpContext = Context.GetHttpContext();

        UserId.TryParse(httpContext.Request.Query["sender"], out UserId senderId);


        IUser sender = (IUser)_userRepository.GetByIdAsync(senderId);

        ChatMessage chatMessage = new ChatMessage(message,DateTime.Now,senderId,groupName);
        Message sentMessage = new Message();
        sentMessage.SenderId = senderId;
        sentMessage.Text = message;
        sentMessage.Time = DateTime.UtcNow;

        _messageService.AddMessage(groupName.ToString(), sentMessage);

        // Broadcast the message to the conversation group
        return Clients.Group(groupName.ToString()).SendAsync("ReceiveMessage", sender.Id, message);
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
