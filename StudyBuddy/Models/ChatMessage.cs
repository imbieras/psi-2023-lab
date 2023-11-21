using StudyBuddy.ValueObjects;

namespace StudyBuddy.Models;

public class ChatMessage
{
    public ChatMessage() {}

    public ChatMessage(string content, DateTime timestamp, UserId senderUserId, Guid conversationId)
    {
        Content = content;
        Timestamp = timestamp;
        SenderUserId = senderUserId;
        ConversationId = conversationId;
    }

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Content { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public UserId SenderUserId { get; set; }

    public Guid ConversationId { get; set; } = Guid.NewGuid();
}
