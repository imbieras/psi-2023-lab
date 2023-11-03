using StudyBuddy.Abstractions;
using StudyBuddy.Models;

namespace StudyBuddy.Controllers.ChatController
{
    public class ChatViewModel
    {
        public IUser CurrentUser { get; set; }
        public IUser OtherUser { get; set; }

        public List<IUser> Matches { get; set; }

        public List<Message> messages { get; set; }
    }

}

