using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudyBuddy.Data.Repositories.ChatRepository;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Services
{
    public class ChatService
    {
        private readonly ChatRepository _chatRepository;

        public ChatService(ChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesByConversationAsync(Guid conversationId)
        {
            return await _chatRepository.GetMessagesByConversationAsync(conversationId);
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesByUserIdsAsync(UserId userId1, UserId userId2)
        {
            return await _chatRepository.GetMessagesByUserIdsAsync(userId1, userId2);
        }

        public async Task<ChatMessage> GetMessageByIdAsync(Guid messageId)
        {
            return await _chatRepository.GetMessageByIdAsync(messageId);
        }

        public async Task AddMessageAsync(ChatMessage message)
        {
            await _chatRepository.AddMessageAsync(message);
        }

        public async Task DeleteMessageAsync(Guid messageId)
        {
            await _chatRepository.DeleteMessageAsync(messageId);
        }
    }
}
