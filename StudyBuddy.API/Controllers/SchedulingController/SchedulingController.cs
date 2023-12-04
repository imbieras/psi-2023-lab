using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyBuddy.API.Services.SchedulingService;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;
using StudyBuddy.Shared.DTOs;
namespace StudyBuddy.API.Controllers.SchedulingController;

[ApiController]
[Route("api/v1/scheduling")]
public class SchedulingController : ControllerBase
{
    private readonly ISchedulingService _schedulingService;

    public SchedulingController(ISchedulingService schedulingService)
    {
        _schedulingService = schedulingService;
    }

    [HttpGet]
    public async Task<List<Event>> GetEvents([FromQuery] DateTime start, [FromQuery] DateTime end, [FromQuery] Guid userId)
    {
        return await _schedulingService.GetEventsByUserIdAsync(userId, start, end);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent([FromRoute] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Event? @event = await _schedulingService.GetEventByIdAsync(id);

        if (@event == null)
        {
            return NotFound();
        }

        return Ok(@event);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutEvent([FromRoute] int id, [FromBody] Event @event)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (@event.Id != id)
        {
            return BadRequest();
        }

        try
        {
            await _schedulingService.UpdateEventAsync(@event);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await EventExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpPut("{id}/move")]
    public async Task<IActionResult> MoveEvent([FromRoute] int id, [FromBody] EventMoveDto param)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var @event = await _schedulingService.GetEventByIdAsync(id);
        if (@event == null)
        {
            return NotFound();
        }

        @event.Start = param.Start;
        @event.End = param.End;

        try
        {
            await _schedulingService.UpdateEventAsync(@event);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await EventExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpPut("{id}/color")]
    public async Task<IActionResult> SetEventColor([FromRoute] int id, [FromBody] EventColorDto param)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var @event = await _schedulingService.GetEventByIdAsync(id);
        if (@event == null)
        {
            return NotFound();
        }

        @event.Color = param.Color;

        try
        {
            await _schedulingService.UpdateEventAsync(@event);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await EventExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> PostEvent([FromBody] Event @event)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _schedulingService.CreateEventAsync(@event);

        return CreatedAtAction("GetEvent", new { id = @event.Id }, @event);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent([FromRoute] int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _schedulingService.DeleteEventAsync(id);

        if (!await EventExists(id))
        {
            return NotFound();
        }

        return Ok();
    }

    private async Task<bool> EventExists(int id)
    {
        return await _schedulingService.GetEventByIdAsync(id) != null;
    }
}
