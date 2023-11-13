using NSubstitute;
using NSubstitute.ExceptionExtensions;
using StudyBuddy.Data.Repositories.ChatRepository;
using StudyBuddy.Models;
using StudyBuddy.Services.ChatService;
using StudyBuddy.Utilities;
using StudyBuddy.ValueObjects;

namespace StudyBuddyTests.Services;

public class ChatServiceTests
{
    private readonly IChatRepository _chatRepository;
    private readonly ChatService _sut;

    public ChatServiceTests()
    {
        _chatRepository = Substitute.For<IChatRepository>();
        _sut = new ChatService(_chatRepository);
    }

    [Fact]
    public async Task GetMessagesByConversationAsync_ExistingConversation_Returns_Messages()
    {
        Guid user1Guid = Guid.Parse("00000000-0000-0000-0000-111111111111");
        UserId user1Id = UserId.From(user1Guid);
        Guid user2Guid = Guid.Parse("00000000-0000-0000-0000-222222222222");
        UserId user2Id = UserId.From(user2Guid);

        Guid conversationId = ConversationIdHelper.GetGroupId(user1Guid, user2Guid);

        List<ChatMessage> expected = new()
        {
            new ChatMessage(
            "Hello",
            DateTime.Now, user1Id,
            ConversationIdHelper.GetGroupId(user1Guid, user2Guid)
            ),
            new ChatMessage(
            "Hi",
            DateTime.Now, user2Id,
            ConversationIdHelper.GetGroupId(user2Guid, user1Guid)
            )
        };

        _chatRepository.GetMessagesByConversationAsync(conversationId).Returns(expected);

        IEnumerable<ChatMessage> result = await _sut.GetMessagesByConversationAsync(conversationId);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetMessagesByConversationAsync_NonExistingConversation_Returns_EmptyList()
    {
        Guid user1Guid = Guid.Parse("00000000-0000-0000-0000-111111111111");
        Guid user2Guid = Guid.Parse("00000000-0000-0000-0000-222222222222");

        Guid conversationId = ConversationIdHelper.GetGroupId(user1Guid, user2Guid);

        List<ChatMessage> expected = new();

        _chatRepository.GetMessagesByConversationAsync(conversationId).Returns(expected);

        IEnumerable<ChatMessage> result = await _sut.GetMessagesByConversationAsync(conversationId);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetMessageByIdAsync_ExistingMessage_Returns_Message()
    {
        Guid messageId = Guid.Parse("00000000-0000-0000-0000-111111111111");
        ChatMessage expected = new("Hello", DateTime.Now, UserId.From(Guid.NewGuid()), Guid.NewGuid());

        _chatRepository.GetMessageByIdAsync(messageId).Returns(expected);

        ChatMessage result = await _sut.GetMessageByIdAsync(messageId);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetMessageByIdAsync_NonExistingMessage_Throws_InvalidOperationException()
    {
        Guid messageId = Guid.Parse("00000000-0000-0000-0000-111111111111");

        _chatRepository.GetMessageByIdAsync(messageId).Throws(new InvalidOperationException());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.GetMessageByIdAsync(messageId));
    }

    [Fact]
    public async Task AddMessageAsync_Adds_Message()
    {
        ChatMessage message = new("Hello", DateTime.Now, UserId.From(Guid.NewGuid()), Guid.NewGuid());

        await _sut.AddMessageAsync(message);

        await _chatRepository.Received().AddMessageAsync(message);
    }

    [Fact]
    public async Task DeleteMessageAsync_Deletes_Message()
    {
        Guid messageId = Guid.Parse("00000000-0000-0000-0000-111111111111");

        await _sut.DeleteMessageAsync(messageId);

        await _chatRepository.Received().DeleteMessageAsync(messageId);
    }

}
