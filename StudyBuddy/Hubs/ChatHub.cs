using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.SignalR;
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


    public override Task OnConnectedAsync()
    {
        HttpContext httpContext = Context.GetHttpContext();

        string receiver = httpContext.Request.Query["userid"];
        // string sender = _userService.GetCurrentUserId().ToString();
        string sender = httpContext.Request.Query["sender"];


        Console.WriteLine("OUTSIDE GROUP SENDER:" + sender + "    " + _userService.GetCurrentUserId);


        if (!string.IsNullOrEmpty(sender))
        {
            Groups.AddToGroupAsync(Context.ConnectionId, sender);
        }


        if (!string.IsNullOrEmpty(receiver))
        {
            Groups.AddToGroupAsync(Context.ConnectionId, receiver);
        }

        Console.WriteLine("Context.ConnectionId: " + Context.ConnectionId + "\nSENDER:" + sender + "\nRECEIVER " + receiver);

        return base.OnConnectedAsync();
    }
    public Task SendMessageToGroup(string receiver, string message)
    {
        return Clients.Group(receiver).SendAsync("ReceiveMessage"
            , Context.User.Identity.Name, message);
    }

    public async Task SendMessage(string user, string message)
    {
        await Console.Out.WriteLineAsync("SendMessage " + user + " " + message);
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task ReceiveMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }


}
