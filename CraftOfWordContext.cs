using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CraftOfWord
{
    public class CraftOfWordContext : DbContext
    {
        public CraftOfWordContext(DbContextOptions<CraftOfWordContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Tournament> Tournament { get; set; }
        public DbSet<Round> Round { get; set; }
        public DbSet<Word> Word { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Many-to-Many relationship between Users and Tournaments
            modelBuilder.Entity<Tournament>()
                .HasMany(t => t.Participants)
                .WithMany(u => u.Tournaments)
                .UsingEntity(j => j.ToTable("TournamentParticipants"));
            modelBuilder.Entity<Tournament>()
                .HasOne(t => t.Winner)
                .WithMany() // No navigation property on User
                .HasForeignKey(t => t.WinnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many relationship between Tournament and Rounds
            modelBuilder.Entity<Tournament>()
                .HasMany(t => t.Rounds)
                .WithOne(r => r.Tournament)
                .HasForeignKey(r => r.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many relationship between Round and Words
            modelBuilder.Entity<Round>()
                .HasMany(r => r.Words)
                .WithOne(w => w.Round)
                .HasForeignKey(w => w.RoundId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many relationship between User and Words
            modelBuilder.Entity<User>()
                .HasMany(u => u.Words)
                .WithOne(w => w.User)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        }

        // Helper method for password hashing
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
