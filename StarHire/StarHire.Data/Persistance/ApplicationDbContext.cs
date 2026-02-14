using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StarHire.Models.Domain.Entities;

namespace StarHire.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Application> Applications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure decimal precision to avoid silent truncation
            builder.Entity<Job>()
                .Property(j => j.Salary)
                .HasPrecision(18, 2);


            // Application
            builder.Entity<Application>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.HasOne(a => a.Job)
                      .WithMany(j => j.Applications)
                      .HasForeignKey(a => a.JobId)
                      .OnDelete(DeleteBehavior.Restrict); // ❗ спира cascade цикъл

                entity.HasOne(a => a.Alien)
                      .WithMany()
                      .HasForeignKey(a => a.AlienId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Job
            builder.Entity<Job>(entity =>
            {
                entity.HasKey(j => j.Id);

                entity.Property(j => j.Title)
                      .IsRequired();

                entity.Property(j => j.Description)
                      .IsRequired();

                entity.Property(j => j.Salary)
                      .HasColumnType("decimal(18,2)");

                entity.HasOne(j => j.Employeer)
                      .WithMany()
                      .HasForeignKey(j => j.EmployeerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
