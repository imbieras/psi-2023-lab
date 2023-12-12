using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StudyBuddy.API.Extensions;
using StudyBuddy.Shared.Models;
using StudyBuddy.Shared.ValueObjects;

namespace StudyBuddy.API.Data;

public class StudyBuddyDbContext : DbContext
{
    public StudyBuddyDbContext(DbContextOptions<StudyBuddyDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<MatchRequest> MatchRequests { get; set; } = null!;

    public DbSet<Match> Matches { get; set; } = null!;

    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

    public DbSet<UserSeenProfile> UserSeenProfiles { get; set; } = null!;

    public DbSet<Event?> Schedules { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.UseValueConverterForType(typeof(UserId), UserIdConverter.UserIdToStringConverter);
        modelBuilder.UseValueConverterForType(typeof(Guid), GuidConverter.GuidToStringConverter);
        modelBuilder.UseValueConverterForType(typeof(List<string>), StringListConverter.StringListToStringConverter);

        // Configure Users
        modelBuilder.Entity<User>().OwnsOne(u => u.Traits);

        // Configure Matches
        modelBuilder.Entity<MatchRequest>().HasKey(mr => new { mr.RequesterId, mr.RequestedId });

        modelBuilder.Entity<Match>()
            .HasKey(m => new { m.User1Id, m.User2Id });

        modelBuilder.Entity<Match>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.User1Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.User2Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasIndex(m => new { m.User1Id, m.User2Id })
            .IsUnique();

        // Configure the ChatMessage
        modelBuilder.Entity<ChatMessage>()
            .HasKey(m => m.Id);

        modelBuilder.Entity<ChatMessage>()
            .HasIndex(m => m.ConversationId);

        // Configure UserSeenProfiles
        modelBuilder.Entity<UserSeenProfile>()
            .HasKey(usp => new { usp.UserId, SeenProfileId = usp.SeenUserId });

        modelBuilder.Entity<UserSeenProfile>()
            .HasIndex(usp => usp.UserId);

        modelBuilder.Entity<UserSeenProfile>()
            .HasIndex(usp => usp.SeenUserId);

        modelBuilder.Entity<UserSeenProfile>()
            .Property(usp => usp.UserId)
            .HasConversion(UserIdConverter.UserIdToStringConverter);

        modelBuilder.Entity<UserSeenProfile>()
            .Property(usp => usp.SeenUserId)
            .HasConversion(UserIdConverter.UserIdToStringConverter);

        modelBuilder.Entity<UserSeenProfile>()
            .Property(usp => usp.Timestamp);

        modelBuilder.Entity<Event>()
            .HasKey(e => e.Id);
    }
}

public static class UserIdConverter
{
    public static readonly ValueConverter<UserId, string> UserIdToStringConverter = new(
        v => v.ToString() ?? string.Empty,
        v => (UserId)Guid.Parse(v));
}

public static class GuidConverter
{
    public static readonly ValueConverter<Guid, string> GuidToStringConverter = new(
        v => v.ToString(),
        v => Guid.Parse(v));
}

public static class StringListConverter
{
    public static readonly ValueConverter<List<string>, string> StringListToStringConverter = new(
        v => string.Join(",", v),
        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
}
