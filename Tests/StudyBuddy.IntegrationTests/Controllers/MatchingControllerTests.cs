using Microsoft.AspNetCore.Mvc.Testing;

namespace StudyBuddy.Tests.Controllers;

public class MatchingControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public MatchingControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetMatchHistory()
    {
        var client = _factory.CreateClient();

        var userId = Guid.Parse("8ec3036c-b5dd-429d-92ef-0cf618c0c306");

        var response = await client.GetAsync($"/api/v1/matching/match-history/{userId}");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetIsMatched()
    {
        var client = _factory.CreateClient();

        var userId = Guid.Parse("8ec3036c-b5dd-429d-92ef-0cf618c0c306");
        var otherUserId = Guid.Parse("f1f577ed-26a3-49d8-aa6e-caab6b57327b");

        var response = await client.GetAsync($"/api/v1/matching/is-matched/{userId}/{otherUserId}");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetIsRequestedMatch()
    {
        var client = _factory.CreateClient();

        var userId = Guid.Parse("f1f577ed-26a3-49d8-aa6e-caab6b57327b");
        var otherUserId = Guid.Parse("1f18f062-3b22-45b4-ac5b-b93c338a92ec");

        var response = await client.GetAsync($"/api/v1/matching/is-requested-match/{userId}/{otherUserId}");

        response.EnsureSuccessStatusCode();
    }
}
