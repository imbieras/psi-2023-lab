using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StudyBuddy.Extensions;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data;

public class StudyBuddyDbContext : DbContext
{
    public StudyBuddyDbContext(DbContextOptions<StudyBuddyDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<MatchRequest> MatchRequests { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Conversation> Conversations { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.UseValueConverterForType(typeof(UserId), UserIdConverter.UserIdToStringConverter);

        modelBuilder.Entity<User>().OwnsOne(u => u.Traits);

        modelBuilder.Entity<MatchRequest>().HasKey(mr => new { mr.RequesterId, mr.RequestedId });

        modelBuilder.Entity<User>()
            .Property(u => u.HobbiesArray)
            .HasColumnName("HobbiesArray");

        modelBuilder.Entity<User>()
            .Property(u => u.UsedIndexesArray)
            .HasColumnName("UsedIndexesArray");

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

        // Configure the Conversation
        modelBuilder.Entity<Conversation>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Conversation>()
            .HasIndex(c => new { c.User1Id, c.User2Id }).IsUnique();

        // Configure the ChatMessage
        modelBuilder.Entity<ChatMessage>()
            .HasKey(m => m.Id);

        modelBuilder.Entity<ChatMessage>()
            .HasIndex(m => m.ConversationId);
    }
}

public static class UserIdConverter
{
    public static readonly ValueConverter<UserId, string> UserIdToStringConverter = new(
        v => v.ToString() ?? string.Empty,
        v => (UserId)Guid.Parse(v));
}
