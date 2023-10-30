using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StudyBuddy.Models;
using StudyBuddy.ValueObjects;

namespace StudyBuddy.Data
{
    public class StudyBuddyDbContext : DbContext
    {
        public StudyBuddyDbContext(DbContextOptions<StudyBuddyDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Hobby> Hobbies { get; set; }
        public DbSet<UserHobby> UserHobbies { get; set; }
        public DbSet<MatchRequest> MatchRequests { get; set; }
        public DbSet<Match> Matches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().OwnsOne(u => u.Traits);

            // Global type conversion for UserId
            var userIdProperties = modelBuilder
                .Model
                .GetEntityTypes()
                .SelectMany(e => e.GetProperties())
                .Where(p => p.ClrType == typeof(UserId))
                .ToList();

            foreach (var property in userIdProperties)
            {
                property.SetColumnType("text");
                property.SetValueConverter(new ValueConverter<UserId, string>(
                    v => v.ToString() ?? string.Empty,
                    v => (UserId)Guid.Parse(v ?? string.Empty)
                ));
            }
        }
    }
}
