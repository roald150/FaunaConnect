using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Models;

namespace FaunaConnect2.Api.Data
{
    public class FaunaDbContext : DbContext
    {
        public FaunaDbContext(DbContextOptions<FaunaDbContext> options) : base(options)
        {
        }

        // Dit worden de echte tabellen in SQL Server
        public DbSet<User> Users { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<HuntingGround> HuntingGrounds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Hier leggen we de 1-op-veel relatie uit: Een User heeft VEEL registraties.
            modelBuilder.Entity<Registration>()
                .HasOne(r => r.User)
                .WithMany(u => u.Registrations)
                .HasForeignKey(r => r.UserId);
        }
    }
}