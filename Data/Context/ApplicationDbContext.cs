using Microsoft.EntityFrameworkCore;
using RouletteTechTest.API.Models.Entities;

namespace RouletteTechTest.API.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
      
        }
    }
}