using Microsoft.EntityFrameworkCore;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data.Repositories.ChatRepository;

public class ChatRepository
{
    private readonly StudyBuddyDbContext _context;

    public ChatRepository(StudyBuddyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ChatMessage>> GetMessagesByConversationAsync(Guid conversationId)
    {
        return await _context.ChatMessages
            .Where(m => m.ConversationId == conversationId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ChatMessage>> GetMessagesByUserIdsAsync(UserId userId1, UserId userId2)
    {
        Guid conversationId = GetConversationIdByUserIds(userId1, userId2);
        return await _context.ChatMessages
            .Where(m => m.ConversationId == conversationId)
            .ToListAsync();
    }

    public async Task<ChatMessage> GetMessageByIdAsync(Guid messageId)
    {
        return await _context.ChatMessages
            .Where(m => m.Id == messageId)
            .FirstOrDefaultAsync() ?? throw new InvalidOperationException();
    }

    public Guid GetConversationIdByUserIds(UserId userId1, UserId userId2)
    {
        var conversation = _context.Conversations
            .FirstOrDefault(c => c.User1Id == userId1 && c.User2Id == userId2);
        if (conversation == null)
        {
            conversation = new Conversation(userId1, userId2);
            _context.Conversations.Add(conversation);
            _context.SaveChanges();
        }
        return conversation.Id;
    }

    public async Task AddMessageAsync(ChatMessage message)
    {
        _context.ChatMessages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteMessageAsync(Guid messageId)
    {
        var message = await GetMessageByIdAsync(messageId);
        _context.ChatMessages.Remove(message);
        await _context.SaveChangesAsync();
    }
}
