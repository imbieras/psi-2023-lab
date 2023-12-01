using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.Models;

namespace StudyBuddy.Controllers.ChatController;

public class ChatViewModel
{
    public IUser CurrentUser { get; set; } = null!;

    public IUser OtherUser { get; set; } = null!;

    public List<IUser?> Matches { get; set; } = null!;

    public List<ChatMessage> Messages { get; set; } = null!;

    public Guid GroupName { get; set; }
}
