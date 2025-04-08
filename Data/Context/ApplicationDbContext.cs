using Microsoft.EntityFrameworkCore;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Round> Rounds { get; set; }
        public DbSet<Bet> Bets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                if (!Database.IsSqlite())
                {
                    modelBuilder.Entity<User>()
                        .Property(u => u.UserName)
                        .HasColumnType("nvarchar(max)")
                        .UseCollation("SQL_Latin1_General_CP1_CI_AS");
                }
                else
                {
                    modelBuilder.Entity<User>()
                        .Property(u => u.UserName)
                        .HasColumnType("TEXT");
                }

                entity.Property(u => u.Balance)
                      .HasColumnType("decimal(18,2)");
            });

            // Relación muchos a muchos: Session <-> Players (User)
            modelBuilder.Entity<Session>()
                .HasMany(s => s.Players)
                .WithMany(u => u.Sessions)
                .UsingEntity(j => j.ToTable("SessionPlayers"));

            // Relación 1 a muchos: Session -> Round
            modelBuilder.Entity<Session>()
                .HasMany(s => s.Rounds)
                .WithOne(r => r.Session)
                .HasForeignKey(r => r.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relación 1 a muchos: Round -> Bet
            modelBuilder.Entity<Round>()
                .HasMany(r => r.Bets)
                .WithOne(b => b.Round)
                .HasForeignKey(b => b.RoundId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Round>()
                .OwnsOne(r => r.Result);

            modelBuilder.Entity<Bet>()
                .Property(b => b.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Bet>()
                .Property(b => b.Prize)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<User>()
                .Property(u => u.Balance)
                .HasColumnType("decimal(18,2)");
        }
    }
}