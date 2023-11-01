using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.SignalR;

namespace StudyBuddy.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string senderId, string userId, string message)
    {
        Console.WriteLine($"SendMessage: SenderId={senderId}, ReceiverId={userId}, Message={message}");
        await Clients.User(userId).SendAsync("ReceiveMessage", senderId, message);
    }


    public async Task ReceiveMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }


}
