using Microsoft.AspNetCore.Mvc;
using StudyBuddy.API.Models;

namespace StudyBuddy.API.Controllers.UtilityController;

[ApiController]
[Route("api/v1/utility")]
public class UtilityController
{
    [HttpGet("total-users")]
    public Task<IActionResult> GetTotalUsers()
    {
        int totalUsers = UserCounter.TotalUsers;
        return Task.FromResult<IActionResult>(new OkObjectResult(totalUsers));
    }
}
