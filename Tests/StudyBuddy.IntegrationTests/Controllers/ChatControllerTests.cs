using Microsoft.AspNetCore.Mvc.Testing;


namespace StudyBuddy.Tests.Controllers;

public class ChatControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ChatControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMessagesForConversation()
    {
        var client = _factory.CreateClient();

        var conversationId = Guid.Parse("7f367481-937e-0b45-3881-c65d7397f17d");

        var response = await client.GetAsync($"/api/v1/chat/conversations/{conversationId}/messages");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetMessageById()
    {
        var client = _factory.CreateClient();

        var messageId = Guid.Parse("711d2f3c-44e0-4726-8084-761f0d6097bf");

        var response = await client.GetAsync($"/api/v1/chat/messages/{messageId}");

        response.EnsureSuccessStatusCode();
    }
}
