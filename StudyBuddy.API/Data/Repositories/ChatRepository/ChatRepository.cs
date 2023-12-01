using Microsoft.EntityFrameworkCore;
using StudyBuddy.Shared.Models;

namespace StudyBuddy.API.Data.Repositories.ChatRepository;

public class ChatRepository : IChatRepository
{
    private readonly StudyBuddyDbContext _context;
    private readonly ILogger<ChatRepository> _logger;

    public ChatRepository(StudyBuddyDbContext context, ILogger<ChatRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ChatMessage>> GetMessagesByConversationAsync(Guid conversationId) =>
        await _context.ChatMessages
            .Where(m => m.ConversationId == conversationId)
            .ToListAsync();

    public async Task<ChatMessage> GetMessageByIdAsync(Guid messageId)
    {
        ChatMessage? chatMessage = await _context.ChatMessages.Where(m => m.Id == messageId).FirstOrDefaultAsync();

        if (chatMessage != null)
        {
            return chatMessage;
        }

        _logger.LogWarning("Message not found");
        throw new InvalidOperationException("Message not found");
    }

    public async Task AddMessageAsync(ChatMessage message)
    {
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMessageAsync(Guid messageId)
    {
        ChatMessage message = await GetMessageByIdAsync(messageId);
        _context.ChatMessages.Remove(message);
        await _context.SaveChangesAsync();
    }
}
