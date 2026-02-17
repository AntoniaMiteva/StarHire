using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StarHire.Models.Domain.Entities;

namespace StarHire.Data
{
   
        public class ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
            {
            }

            public DbSet<Job> Jobs { get; set; }
            public DbSet<Application> Applications { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);


                modelBuilder.Entity<Job>()
                    .Property(j => j.Salary)
                    .HasPrecision(18, 2);

            modelBuilder.Entity<Application>()
               .HasOne(a => a.Job)
               .WithMany(j => j.Applications)
               .HasForeignKey(a => a.JobId)
               .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Application>()
                .HasOne(a => a.Alien)
                .WithMany()
                .HasForeignKey(a => a.AlienId)
                .OnDelete(DeleteBehavior.Restrict);
        }
        }
    }

