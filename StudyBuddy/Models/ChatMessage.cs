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
        this.Content = content;
        this.Timestamp = timestamp;
        this.SenderUserId = senderUserId;
        this.ConversationId = conversationId;
    }

    public ChatMessage(){}
}
