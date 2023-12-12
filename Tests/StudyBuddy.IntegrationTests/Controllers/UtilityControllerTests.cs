using Microsoft.AspNetCore.Mvc.Testing;

namespace StudyBuddy.Tests.Controllers;

public class UtilityControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UtilityControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task TotalUsers()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/v1/utility/total-users");

        response.EnsureSuccessStatusCode();

        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
    }
}
