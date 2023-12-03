using Microsoft.AspNetCore.Mvc;
using StudyBuddy.API.Services.UserService;
using StudyBuddy.Shared.Abstractions;
using StudyBuddy.Shared.DTOs;
using StudyBuddy.Shared.Exceptions;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Controllers.UserController;

[ApiController]
[Route("api/v1/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService) => _userService = userService;

    [HttpGet]
    public async Task<IActionResult> GetAllUsersAsync() => Ok(await _userService.GetAllUsersAsync());

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        IUser? user = await _userService.GetUserByIdAsync(UserId.From(userId));
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("{userId:guid}/hobbies")]
    public async Task<IActionResult> GetUserHobbies(Guid userId)
    {
        List<string>? hobbies = await _userService.GetHobbiesById(UserId.From(userId));
        return Ok(hobbies);
    }

    [HttpGet("by-username/{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        IUser? user = await _userService.GetUserByUsernameAsync(username);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("ban/{userId:guid}")]
    public async Task<IActionResult> BanUser(Guid userId)
    {
        await _userService.BanUserAsync(UserId.From(userId));
        return Ok();
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] ProfileDto profileDto)
    {
        try
        {
            await _userService.RegisterUserAsync(profileDto);
            return Ok();
        }
        catch (UsernameAlreadyTakenException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidUsernameException e)
        {
            return BadRequest(e.Message);
        }
        catch (InvalidPasswordException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return Ok(e.Message);
        }
    }

    [HttpPut("{userId:guid}/update")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto updateUserDto)
    {
        UserId parseUserId = UserId.From(userId);

        IUser? existingUser = await _userService.GetUserByIdAsync(parseUserId);

        if (existingUser == null)
        {
            return NotFound("User not found");
        }

        await _userService.UpdateAsync(parseUserId, updateUserDto);

        return NoContent();
    }


    [HttpGet("random")]
    public async Task<IActionResult> GetRandomUser()
    {
        IUser? randomUser = await _userService.GetRandomUserAsync();
        if (randomUser == null)
        {
            return NotFound();
        }

        return Ok(randomUser);
    }

    [HttpGet("{userId:guid}/ultimate-seen-user")]
    public async Task<IActionResult> GetUltimateSeenUser(Guid userId)
    {
        IUser? user = await _userService.GetUltimateSeenUserAsync(UserId.From(userId));
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("{userId:guid}/penultimate-seen-user")]
    public async Task<IActionResult> GetPenultimateSeenUser(Guid userId)
    {
        IUser? user = await _userService.GetPenultimateSeenUserAsync(UserId.From(userId));
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("{userId:guid}/not-seen-any-user")]
    public async Task<IActionResult> IsUserNotSeenAnyUser(Guid userId)
    {
        bool isUserNotSeenAnyUser = await _userService.IsUserNotSeenAnyUserAsync(UserId.From(userId));
        return Ok(isUserNotSeenAnyUser);
    }

    [HttpGet("{userId:guid}/has-seen-user/{otherUserId:guid}")]
    public async Task<IActionResult> IsUserSeen(Guid userId, Guid otherUserId)
    {
        bool isUserSeen = await _userService.IsUserSeenAsync(UserId.From(userId), UserId.From(otherUserId));
        return Ok(isUserSeen);
    }

    [HttpPost("{userId:guid}/user-seen/{otherUserId:guid}")]
    public async Task<IActionResult> UserSeen(Guid userId, Guid otherUserId)
    {
        await _userService.UserSeenAsync(UserId.From(userId), UserId.From(otherUserId));
        return Ok();
    }
}
