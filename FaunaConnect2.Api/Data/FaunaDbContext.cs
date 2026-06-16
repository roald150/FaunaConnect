using Microsoft.EntityFrameworkCore;
using FaunaConnect2.Api.Models;

namespace FaunaConnect2.Api.Data
{
    public class FaunaDbContext : DbContext
    {
        public FaunaDbContext(DbContextOptions<FaunaDbContext> options) : base(options)
        {
        }
    
        public DbSet<User> Users { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<HuntingGround> HuntingGrounds { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<AnimalSpecies> AnimalSpecies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.User)
                .WithMany(u => u.Registrations)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<DamageReport>()
                .HasOne(d => d.User)
                .WithMany(u => u.DamageReports)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.LinkedJager)
                .WithMany(u => u.LinkedFarmers)
                .HasForeignKey(u => u.LinkedJagerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}