using StudyBuddy.API.Data.Repositories.SchedulingRepository;
using StudyBuddy.Shared.Models;

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

    public async Task<List<DateTime>> GetOverlapByUserIds(Guid userId1, Guid userId2, DateTime start, DateTime end)
    {
        List<Event> user1Events = await _schedulingRepository.GetEventsByUserIdAsync(userId1, start, end);
        List<Event> user2Events = await _schedulingRepository.GetEventsByUserIdAsync(userId2, start, end);

        List<DateTime> overlapDates = new List<DateTime>();

        foreach (DateTime overlapDate in from event1 in user1Events
                 from event2 in user2Events
                 where event1.Start.Date <= event2.End.Date && event1.End.Date >= event2.Start.Date
                 select event1.Start.Date > event2.Start.Date ? event1.Start.Date : event2.Start.Date
                 into overlapDate
                 where !overlapDates.Contains(overlapDate)
                 select overlapDate)
        {
            overlapDates.Add(overlapDate);
        }

        return overlapDates;
    }

    public async Task<Event> DeleteEventAsync(int id) => await _schedulingRepository.DeleteEventAsync(id);
}
