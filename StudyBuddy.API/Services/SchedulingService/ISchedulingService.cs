using StudyBuddy.Shared.DTOs;
using StudyBuddy.Shared.Models;

namespace StudyBuddy.API.Services.SchedulingService;

public interface ISchedulingService
{
        Task<List<Event>> GetEventsAsync(DateTime start, DateTime end);
        Task<List<Event>> GetEventsByUserIdAsync(Guid userId, DateTime start, DateTime end);
        Task<Event?> GetEventByIdAsync(int id);
        Task<Event> CreateEventAsync(Event @event);
        Task<Event> UpdateEventAsync(Event @event);
        Task<Event> DeleteEventAsync(int id);
        Task<List<DateTime>> GetOverlapByUserIds(OverlapDto overlapDto);
}
