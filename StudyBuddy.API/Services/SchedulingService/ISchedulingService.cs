using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Services.SchedulingService;

public interface ISchedulingService
{
        Task<List<Event>> GetEventsAsync(DateTime start, DateTime end);
        Task<List<Event>> GetEventsByUserIdAsync(Guid userId, DateTime start, DateTime end);
        Task<Event?> GetEventByIdAsync(int id);
        Task<Event> CreateEventAsync(Event @event);
        Task<Event> UpdateEventAsync(Event @event);
        Task<Event> DeleteEventAsync(int id);
        Task<List<DateTime>> GetOverlapByUserIds(Guid userId1, Guid userId2, DateTime start, DateTime end);
}
