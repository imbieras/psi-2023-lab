using Microsoft.AspNetCore.Mvc.Testing;

namespace StudyBuddy.Tests.Controllers;

public class SchedulingControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SchedulingControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetSchedules()
    {
        var client = _factory.CreateClient();

        var startDate = DateTime.Now;
        var endDate = DateTime.Now.AddDays(7);
        var userId = Guid.Parse("8ec3036c-b5dd-429d-92ef-0cf618c0c306");

        var response = await client.GetAsync($"/api/v1/scheduling?startDate={startDate}&endDate={endDate}&userId={userId}");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetScheduleById()
    {
        var client = _factory.CreateClient();

        const int scheduleId = 17;

        var response = await client.GetAsync($"/api/v1/scheduling/{scheduleId}");

        response.EnsureSuccessStatusCode();
    }
}
