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

            modelBuilder.Entity<UserSkill>()
                .HasKey(us => new { us.UserId, us.SkillId });

            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserId);

            modelBuilder.Entity<UserSkill>()
                .HasOne(us => us.Skill)
                .WithMany(s => s.UserSkills)
                .HasForeignKey(us => us.SkillId);

            modelBuilder.Entity<Skill>()
                .HasIndex(s => s.NormalizedName)
                .IsUnique();

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Job)
                .WithMany()
                .HasForeignKey(f => f.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<UserSkill> UserSkills { get; set; } = null!;

        public DbSet<Favorite> Favorites { get; set; }
    }
 }

