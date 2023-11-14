using StudyBuddy.Models;

namespace StudyBuddy.Data.Repositories.ChatRepository;

public interface IChatRepository
{
    Task<IEnumerable<ChatMessage>> GetMessagesByConversationAsync(Guid conversationId);
    Task<ChatMessage> GetMessageByIdAsync(Guid messageId);
    Task AddMessageAsync(ChatMessage message);
    Task DeleteMessageAsync(Guid messageId);
}
