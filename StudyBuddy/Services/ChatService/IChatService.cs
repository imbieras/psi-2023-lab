using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services.ChatService;

public interface IChatService
{
    Task<IEnumerable<ChatMessage>> GetMessagesByConversationAsync(Guid conversationId);

    Task<ChatMessage> GetMessageByIdAsync(Guid messageId);

    Task AddMessageAsync(ChatMessage message);

    Task DeleteMessageAsync(Guid messageId);
}
