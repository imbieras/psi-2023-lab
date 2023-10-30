using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.SignalR;

namespace StudyBuddy.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        Console.WriteLine("SendMessage");
        await Clients.All.SendAsync("ReceiveMessage", user, message);

    }

    public async Task ReceiveMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }


}
