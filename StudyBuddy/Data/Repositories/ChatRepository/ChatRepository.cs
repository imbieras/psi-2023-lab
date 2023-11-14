using Microsoft.EntityFrameworkCore;
using StudyBuddy.Models;

namespace StudyBuddy.Data.Repositories.ChatRepository;

public class ChatRepository : IChatRepository
{
    private readonly StudyBuddyDbContext _context;

    public ChatRepository(StudyBuddyDbContext context) => _context = context;

    public async Task<IEnumerable<ChatMessage>> GetMessagesByConversationAsync(Guid conversationId) =>
        await _context.ChatMessages
            .Where(m => m.ConversationId == conversationId)
            .ToListAsync();

    public async Task<ChatMessage> GetMessageByIdAsync(Guid messageId) =>
        await _context.ChatMessages
            .Where(m => m.Id == messageId)
            .FirstOrDefaultAsync() ?? throw new InvalidOperationException();

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
