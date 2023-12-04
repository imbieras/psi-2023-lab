using StudyBuddy.API.Data.Repositories.SchedulingRepository;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;


namespace StudyBuddy.API.Services.SchedulingService;

public class SchedulingService : ISchedulingService
{
    private readonly ISchedulingRepository _schedulingRepository;

    public SchedulingService(ISchedulingRepository schedulingRepository)
    {
        _schedulingRepository = schedulingRepository;
    }

    public async Task<List<Event>> GetEventsAsync(DateTime start, DateTime end)
    {
        return await _schedulingRepository.GetEventsAsync(start, end);
    }

    public async Task<List<Event>> GetEventsByUserIdAsync(Guid userId, DateTime start, DateTime end)
    {
        return await _schedulingRepository.GetEventsByUserIdAsync(userId, start, end);
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        return await _schedulingRepository.GetEventByIdAsync(id);
    }

    public async Task<Event> CreateEventAsync(Event @event)
    {
        return await _schedulingRepository.CreateEventAsync(@event);
    }

    public async Task<Event> UpdateEventAsync(Event @event)
    {
        return await _schedulingRepository.UpdateEventAsync(@event);
    }

    public async Task<Event> DeleteEventAsync(int id) => await _schedulingRepository.DeleteEventAsync(id);
}
