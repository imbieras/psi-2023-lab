using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class ChatMessage
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public UserId SenderUserId { get; set; }
    public Guid ConversationId { get; set; }

    public ChatMessage(string content, DateTime timestamp, UserId senderUserId, Guid conversationId)
    {
        Content = content;
        Timestamp = timestamp;
        SenderUserId = senderUserId;
        ConversationId = conversationId;
    }

    public ChatMessage()
    {
    }
}
