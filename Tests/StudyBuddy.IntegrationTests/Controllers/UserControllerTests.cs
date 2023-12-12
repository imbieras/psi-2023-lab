using Microsoft.AspNetCore.Mvc.Testing;

namespace StudyBuddy.Tests.Controllers;

public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UserControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetUsers()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/user");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetUserById()
    {
        var client = _factory.CreateClient();

        var userId = Guid.Parse("8ec3036c-b5dd-429d-92ef-0cf618c0c306");

        var response = await client.GetAsync($"/api/v1/user/{userId}");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetHobbiesByUserId()
    {
        var client = _factory.CreateClient();

        var userId = Guid.Parse("8ec3036c-b5dd-429d-92ef-0cf618c0c306");

        var response = await client.GetAsync($"/api/v1/user/{userId}/hobbies");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetUserByUsername()
    {
        var client = _factory.CreateClient();

        const string username = "testing1";

        var response = await client.GetAsync($"/api/v1/user/by-username/{username}");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetRandomUser()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/v1/user/random");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetHasSeenUser()
    {
        var client = _factory.CreateClient();

        var userId = Guid.Parse("8ec3036c-b5dd-429d-92ef-0cf618c0c306");
        var otherUserId = Guid.Parse("f1f577ed-26a3-49d8-aa6e-caab6b57327b");

        var response = await client.GetAsync($"/api/v1/user/{userId}/has-seen-user/{otherUserId}");

        response.EnsureSuccessStatusCode();
    }
}
