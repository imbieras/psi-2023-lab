using StudyBuddy.Shared.Models;

namespace StudyBuddy.API.Services.ChatService;

public interface IChatService
{
    Task<IEnumerable<ChatMessage>> GetMessagesByConversationAsync(Guid conversationId);

    Task<ChatMessage> GetMessageByIdAsync(Guid messageId);

    Task AddMessageAsync(ChatMessage message);

    Task DeleteMessageAsync(Guid messageId);
}
