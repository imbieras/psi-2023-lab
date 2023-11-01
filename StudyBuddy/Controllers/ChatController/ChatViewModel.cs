using StudyBuddy.Abstractions;

namespace StudyBuddy.Controllers.ChatController
{
    public class ChatViewModel
    {
        public IUser CurrentUser { get; set; }
        public IUser OtherUser { get; set; }
    }

}

