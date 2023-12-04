using Microsoft.EntityFrameworkCore;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Data.Repositories.SchedulingRepository;

public class SchedulingRepository : ISchedulingRepository
{
    private readonly StudyBuddyDbContext _context;
    private readonly ILogger<SchedulingRepository> _logger;

    public SchedulingRepository(StudyBuddyDbContext context, ILogger<SchedulingRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Event>> GetEventsAsync(DateTime start, DateTime end)
    {
        // TODO: Not too sure what to do with the null situation here
        return await _context.Schedules.ToListAsync();
        // return await _context.Schedules.Where(e => (e.Start >= start && e.End <= end)).ToListAsync();
    }

    public async Task<List<Event>> GetEventsByUserIdAsync(Guid userId, DateTime start, DateTime end)
    {
        return await _context.Schedules.Where(e => e.UserId == userId).ToListAsync();
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        return await _context.Schedules.SingleOrDefaultAsync(e => e != null && e.Id == id);
    }

    public async Task<Event> CreateEventAsync(Event @event)
    {
        _context.Schedules.Add(@event);
        await _context.SaveChangesAsync();
        return @event;
    }

    public async Task<Event> UpdateEventAsync(Event @event)
    {
        _context.Entry(@event).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return @event;
    }

    public async Task<Event> DeleteEventAsync(int id)
    {
        var @event = await _context.Schedules.SingleOrDefaultAsync(e => e != null && e.Id == id);
        if (@event == null)
        {
            throw new ArgumentException("Event not found");
        }

        _context.Schedules.Remove(@event);
        await _context.SaveChangesAsync();
        return @event;
    }
}
