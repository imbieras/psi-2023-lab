using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StudyBuddy.Data;
using StudyBuddy.Data.Repositories.ChatRepository;
using StudyBuddy.Models;
using StudyBuddy.Utilities;
using StudyBuddy.ValueObjects;

namespace StudyBuddyTests.Data.Repositories;

public class ChatRepositoryTests
{
    private StudyBuddyDbContext _dbContext;
    private readonly ChatRepository _sut;

    public ChatRepositoryTests()
    {
        DbContextOptions<StudyBuddyDbContext> options = new DbContextOptionsBuilder<StudyBuddyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new StudyBuddyDbContext(options);

        IEnumerable<ChatMessage> messages = GenerateChatMessages();

        _sut = new ChatRepository(_dbContext);

        _dbContext.ChatMessages.AddRange(messages);

        _dbContext.SaveChanges();
    }

    private static IEnumerable<ChatMessage> GenerateChatMessages()
    {
        Guid user1Guid = Guid.Parse("00000000-0000-0000-0000-111111111111");
        UserId user1Id = UserId.From(user1Guid);
        Guid user2Guid = Guid.Parse("00000000-0000-0000-0000-222222222222");
        UserId user2Id = UserId.From(user2Guid);
        Guid user3Guid = Guid.Parse("00000000-0000-0000-0000-333333333333");

        List<ChatMessage> messages = new()
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
            ),
            new ChatMessage(
            "What's up?",
            DateTime.Now, user1Id,
            ConversationIdHelper.GetGroupId(user1Guid, user3Guid)
            )
        };

        return messages;
    }

    [Fact]
    public async Task GetMessagesByConversationAsync_Returns_AllMessagesByConversation()
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
            conversationId
            ),
            new ChatMessage(
            "Hi",
            DateTime.Now, user2Id,
            conversationId
            )
        };

        List<ChatMessage> actual = (await _sut.GetMessagesByConversationAsync(conversationId)).ToList();

        for (int i = 0; i < expected.Count; i++)
        {
            expected[i].Id = actual[i].Id;
        }

        expected.Should().BeEquivalentTo(actual, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task GetMessageByIdAsync_ExistingMessageId_Returns_MessageById()
    {
        Guid user1Guid = Guid.Parse("00000000-0000-0000-0000-111111111111");
        UserId user1Id = UserId.From(user1Guid);
        Guid user2Guid = Guid.Parse("00000000-0000-0000-0000-222222222222");

        ChatMessage expected = new(
        "Hello",
        DateTime.Now, user1Id,
        ConversationIdHelper.GetGroupId(user1Guid, user2Guid)
        );

        await _sut.AddMessageAsync(expected);

        ChatMessage actual = await _sut.GetMessageByIdAsync(expected.Id);

        expected.Should().BeEquivalentTo(actual, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task GetMessageByIdAsync_NonExistingMessageId_Throws_InvalidOperationException()
    {
        Guid user1Guid = Guid.Parse("00000000-0000-0000-0000-111111111111");
        UserId user1Id = UserId.From(user1Guid);
        Guid user2Guid = Guid.Parse("00000000-0000-0000-0000-222222222222");

        ChatMessage message = new(
        "Hello",
        DateTime.Now, user1Id,
        ConversationIdHelper.GetGroupId(user1Guid, user2Guid)
        );

        await _sut.Invoking(sut => sut.GetMessageByIdAsync(message.Id)).Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task AddMessageAsync_Adds_Message()
    {
        Guid user1Guid = Guid.Parse("00000000-0000-0000-0000-111111111111");
        UserId user1Id = UserId.From(user1Guid);
        Guid user2Guid = Guid.Parse("00000000-0000-0000-0000-222222222222");

        Guid conversationId = ConversationIdHelper.GetGroupId(user1Guid, user2Guid);

        List<ChatMessage> expected = (await _sut.GetMessagesByConversationAsync(conversationId)).ToList();

        ChatMessage message = new("Testing", DateTime.Now, user1Id, conversationId);

        expected.Add(message);

        await _sut.AddMessageAsync(message);

        List<ChatMessage> actual = (await _sut.GetMessagesByConversationAsync(conversationId)).ToList();

        for (int i = 0; i < expected.Count; i++)
        {
            expected[i].Id = actual[i].Id;
        }

        expected.Should().BeEquivalentTo(actual, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task DeleteMessageAsync_ExistingMessageId_Deletes_Message()
    {
        Guid user1Guid = Guid.Parse("00000000-0000-0000-0000-111111111111");
        Guid user2Guid = Guid.Parse("00000000-0000-0000-0000-222222222222");

        Guid conversationId = ConversationIdHelper.GetGroupId(user1Guid, user2Guid);

        List<ChatMessage> expected = (await _sut.GetMessagesByConversationAsync(conversationId)).ToList();

        ChatMessage message = expected[0];

        expected.RemoveAt(0);

        await _sut.DeleteMessageAsync(message.Id);

        List<ChatMessage> actual = (await _sut.GetMessagesByConversationAsync(conversationId)).ToList();

        expected.Should().BeEquivalentTo(actual, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    [Fact]
    public async Task DeleteMessageAsync_NonExistingMessageId_Throws_InvalidOperationException()
    {
        ChatMessage message = new(
        "Hello",
        DateTime.Now, UserId.From(Guid.NewGuid()),
        ConversationIdHelper.GetGroupId(Guid.NewGuid(), Guid.NewGuid())
        );

        await _sut.Invoking(sut => sut.DeleteMessageAsync(message.Id)).Should().ThrowAsync<InvalidOperationException>();
    }
}
