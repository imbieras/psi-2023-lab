using Microsoft.AspNetCore.Mvc;
using StudyBuddy.API.Services.MatchingService;
using StudyBuddy.Shared.DTOs;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Controllers.MatchingController;

[ApiController]
[Route("api/v1/matching")]
public class MatchingController : ControllerBase
{
    private readonly IMatchingService _matchingService;

    public MatchingController(IMatchingService matchingService)
    {
        _matchingService = matchingService;
    }

    [HttpPost("match-users")]
    public async Task<IActionResult> MatchUsers([FromBody] MatchDto matchDto)
    {
        await _matchingService.MatchUsersAsync(matchDto);
        return NoContent();
    }

    [HttpGet("is-matched/{currentUser:guid}/{otherUser:guid}")]
    public async Task<IActionResult> IsMatched(Guid currentUser, Guid otherUser)
    {
        bool isMatched = await _matchingService.IsMatchedAsync(UserId.From(currentUser), UserId.From(otherUser));
        return Ok(isMatched);
    }

    [HttpGet("match-history/{userId:guid}")]
    public async Task<IActionResult> GetMatchHistory(Guid userId)
    {
        var matchHistory = await _matchingService.GetMatchHistoryAsync(UserId.From(userId));
        return Ok(matchHistory);
    }

    [HttpGet("is-requested-match/{currentUser:guid}/{otherUser:guid}")]
    public async Task<IActionResult> IsRequestedMatch(Guid currentUser, Guid otherUser)
    {
        bool isRequestedMatch = await _matchingService.IsRequestedMatchAsync(UserId.From(currentUser), UserId.From(otherUser));
        return Ok(isRequestedMatch);
    }
}
