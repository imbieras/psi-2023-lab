using StudyBuddy.Data.Repositories.ChatRepository;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services.ChatService;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository) => _chatRepository = chatRepository;

    public async Task<IEnumerable<ChatMessage>> GetMessagesByConversationAsync(Guid conversationId) =>
        await _chatRepository.GetMessagesByConversationAsync(conversationId);

    public async Task<IEnumerable<ChatMessage>> GetMessagesByUserIdsAsync(UserId userId1, UserId userId2) =>
        await _chatRepository.GetMessagesByUserIdsAsync(userId1, userId2);

    public async Task<ChatMessage> GetMessageByIdAsync(Guid messageId) =>
        await _chatRepository.GetMessageByIdAsync(messageId);

    public async Task AddMessageAsync(ChatMessage message) => await _chatRepository.AddMessageAsync(message);

    public async Task DeleteMessageAsync(Guid messageId) => await _chatRepository.DeleteMessageAsync(messageId);
}
