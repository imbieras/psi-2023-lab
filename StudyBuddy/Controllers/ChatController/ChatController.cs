using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using StudyBuddy.Hubs;

namespace StudyBuddy.Controllers.ChatController
{
    public class ChatController : Controller
    {

        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;

        }


        public IActionResult Chat()
        {
            _hubContext.Clients.All.SendAsync("CallReceived", 4);
            return View();
        }
    }
}
