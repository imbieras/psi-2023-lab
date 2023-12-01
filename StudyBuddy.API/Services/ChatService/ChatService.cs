using StudyBuddy.API.Data.Repositories.ChatRepository;
using StudyBuddy.Shared.Models;

namespace StudyBuddy.API.Services.ChatService;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;

    public ChatService(IChatRepository chatRepository) => _chatRepository = chatRepository;

    public async Task<IEnumerable<ChatMessage>> GetMessagesByConversationAsync(Guid conversationId) =>
        await _chatRepository.GetMessagesByConversationAsync(conversationId);

    public async Task<ChatMessage> GetMessageByIdAsync(Guid messageId) =>
        await _chatRepository.GetMessageByIdAsync(messageId);

    public async Task AddMessageAsync(ChatMessage message) => await _chatRepository.AddMessageAsync(message);

    public async Task DeleteMessageAsync(Guid messageId) => await _chatRepository.DeleteMessageAsync(messageId);
}
