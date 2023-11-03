using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories.ChatRepository;

public interface IMessageRepository
{
    Task<IEnumerable<ChatMessage>> GetMessagesByConversationAsync(Guid conversationId);
    Task<IEnumerable<ChatMessage>> GetMessagesByUserIdsAsync(UserId userId1, UserId userId2);
    Task<ChatMessage> GetMessageByIdAsync(Guid messageId);
    Guid GetConversationIdByUserIds(UserId userId1, UserId userId2);
    Task AddMessageAsync(ChatMessage message);
    Task DeleteMessageAsync(Guid messageId);
}
