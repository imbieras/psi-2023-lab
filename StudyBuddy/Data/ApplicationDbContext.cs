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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.UseValueConverterForType(typeof(UserId), UserIdConverter.UserIdToStringConverter);

        modelBuilder.Entity<User>().OwnsOne(u => u.Traits);

        modelBuilder.Entity<MatchRequest>().HasKey(mr => mr.RequestId);

        modelBuilder.Entity<User>()
            .Property(u => u.HobbiesArray)
            .HasColumnName("HobbiesArray");

        modelBuilder.Entity<User>()
            .Property(u => u.UsedIndexesArray)
            .HasColumnName("UsedIndexesArray");


        modelBuilder.Entity<Match>()
            .HasOne(m => m.User1)
            .WithMany()
            .HasForeignKey(m => m.User1Id);

        modelBuilder.Entity<Match>()
            .HasOne(m => m.User2)
            .WithMany()
            .HasForeignKey(m => m.User2Id);
    }
}

public static class UserIdConverter
{
    public static readonly ValueConverter<UserId, string> UserIdToStringConverter = new(
        v => v.ToString() ?? string.Empty,
        v => (UserId)Guid.Parse(v));
}
