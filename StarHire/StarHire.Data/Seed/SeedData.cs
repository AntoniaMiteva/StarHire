using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StarHire.Data;
using StarHire.Models.Domain.Entities;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();
        var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

        string[] roles = { "Admin", "Employer", "Alien" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        // Admin user
        var adminEmail = "admin@hire.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var user = new IdentityUser<Guid>
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        // Employer user
        var employerEmail = "employer@hire.com";
        var employerUser = await userManager.FindByEmailAsync(employerEmail);

        if (employerUser == null)
        {
            employerUser = new IdentityUser<Guid>
            {
                UserName = employerEmail,
                Email = employerEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(employerUser, "Employer123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(employerUser, "Employer");
            }
        }

        // Alien user
        var alienEmail = "alien@hire.com";
        var alienUser = await userManager.FindByEmailAsync(alienEmail);

        if (alienUser == null)
        {
            alienUser = new IdentityUser<Guid>
            {
                UserName = alienEmail,
                Email = alienEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(alienUser, "Alien123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(alienUser, "Alien");
            }
        }

        // Seed demo skills FIRST
        if (!await db.Skills.AnyAsync())
        {
            db.Skills.AddRange(
                new Skill { Id = Guid.NewGuid(), Name = "Piloting", NormalizedName = "PILOTING" },
                new Skill { Id = Guid.NewGuid(), Name = "Engineering", NormalizedName = "ENGINEERING" },
                new Skill { Id = Guid.NewGuid(), Name = "Mining", NormalizedName = "MINING" },
                new Skill { Id = Guid.NewGuid(), Name = "Trading", NormalizedName = "TRADING" },
                new Skill { Id = Guid.NewGuid(), Name = "Repair", NormalizedName = "REPAIR" },
                new Skill { Id = Guid.NewGuid(), Name = "Navigation", NormalizedName = "NAVIGATION" },
                new Skill { Id = Guid.NewGuid(), Name = "Astro Navigation", NormalizedName = "ASTRO NAVIGATION" },
                new Skill { Id = Guid.NewGuid(), Name = "Robotics", NormalizedName = "ROBOTICS" },
                new Skill { Id = Guid.NewGuid(), Name = "AI Programming", NormalizedName = "AI PROGRAMMING" },
                new Skill { Id = Guid.NewGuid(), Name = "Cybersecurity", NormalizedName = "CYBERSECURITY" },
                new Skill { Id = Guid.NewGuid(), Name = "Terraforming", NormalizedName = "TERRAFORMING" },
                new Skill { Id = Guid.NewGuid(), Name = "Quantum Mechanics", NormalizedName = "QUANTUM MECHANICS" },
                new Skill { Id = Guid.NewGuid(), Name = "Astrophysics", NormalizedName = "ASTROPHYSICS" },
                new Skill { Id = Guid.NewGuid(), Name = "Diplomacy", NormalizedName = "DIPLOMACY" },
                new Skill { Id = Guid.NewGuid(), Name = "Alien Communication", NormalizedName = "ALIEN COMMUNICATION" },
                new Skill { Id = Guid.NewGuid(), Name = "Laser Weapon Handling", NormalizedName = "LASER WEAPON HANDLING" },
                new Skill { Id = Guid.NewGuid(), Name = "Ship Repair", NormalizedName = "SHIP REPAIR" },
                new Skill { Id = Guid.NewGuid(), Name = "Logistics", NormalizedName = "LOGISTICS" },
                new Skill { Id = Guid.NewGuid(), Name = "Medical Training", NormalizedName = "MEDICAL TRAINING" },
                new Skill { Id = Guid.NewGuid(), Name = "Exploration", NormalizedName = "EXPLORATION" }
            );

            await db.SaveChangesAsync();
        }

        // Seed demo jobs AFTER skills
        if (!await db.Jobs.AnyAsync())
        {
            var skills = await db.Skills.ToListAsync();
            Skill Get(string name) => skills.First(s => s.Name == name);

            var jobs = new List<Job>
            {
                new Job { Id = Guid.NewGuid(), Title = "Space Engineer", Planet = "Mars", Salary = 5000, Description = "Maintain Mars stations.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Engineering").Id }, new JobSkill { SkillId = Get("Robotics").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Space Engineer", Planet = "Venus", Salary = 5200, Description = "Station maintenance.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Engineering").Id }, new JobSkill { SkillId = Get("Repair").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Asteroid Miner", Planet = "Mars", Salary = 7000, Description = "Mining operations.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Mining").Id }, new JobSkill { SkillId = Get("Exploration").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Asteroid Miner", Planet = "Jupiter", Salary = 7200, Description = "Deep asteroid mining.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Mining").Id }, new JobSkill { SkillId = Get("Navigation").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "UFO Pilot", Planet = "Saturn", Salary = 9000, Description = "Cargo transportation.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Piloting").Id }, new JobSkill { SkillId = Get("Navigation").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "UFO Pilot", Planet = "Mars", Salary = 8800, Description = "Interplanetary flights.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Piloting").Id }, new JobSkill { SkillId = Get("Astro Navigation").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Galactic Trader", Planet = "Neptune", Salary = 6500, Description = "Trade coordination.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Trading").Id }, new JobSkill { SkillId = Get("Diplomacy").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Galactic Trader", Planet = "Venus", Salary = 6700, Description = "Interplanetary trade.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Trading").Id }, new JobSkill { SkillId = Get("Logistics").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Space Mechanic", Planet = "Mars", Salary = 4800, Description = "Ship repairs.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Repair").Id }, new JobSkill { SkillId = Get("Ship Repair").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Space Mechanic", Planet = "Jupiter", Salary = 5100, Description = "Engine maintenance.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Repair").Id }, new JobSkill { SkillId = Get("Engineering").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Space Engineer", Planet = "Saturn", Salary = 5400, Description = "Base engineering.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Engineering").Id }, new JobSkill { SkillId = Get("Quantum Mechanics").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Asteroid Miner", Planet = "Neptune", Salary = 7100, Description = "Mineral extraction.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Mining").Id }, new JobSkill { SkillId = Get("Robotics").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "UFO Pilot", Planet = "Venus", Salary = 9200, Description = "Luxury space transport.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Piloting").Id }, new JobSkill { SkillId = Get("Alien Communication").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Galactic Trader", Planet = "Mars", Salary = 6300, Description = "Market expansion.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Trading").Id }, new JobSkill { SkillId = Get("Diplomacy").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Space Mechanic", Planet = "Saturn", Salary = 5000, Description = "Station repairs.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Repair").Id }, new JobSkill { SkillId = Get("Ship Repair").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Space Engineer", Planet = "Neptune", Salary = 5600, Description = "Structural design.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Engineering").Id }, new JobSkill { SkillId = Get("Astrophysics").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Asteroid Miner", Planet = "Saturn", Salary = 7300, Description = "Outer belt mining.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Mining").Id }, new JobSkill { SkillId = Get("Exploration").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "UFO Pilot", Planet = "Jupiter", Salary = 9100, Description = "Freight pilot.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Piloting").Id }, new JobSkill { SkillId = Get("Logistics").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Galactic Trader", Planet = "Jupiter", Salary = 6900, Description = "Trade logistics.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Trading").Id }, new JobSkill { SkillId = Get("Logistics").Id } } },
                new Job { Id = Guid.NewGuid(), Title = "Space Mechanic", Planet = "Venus", Salary = 4700, Description = "Equipment diagnostics.", EmployerId = employerUser!.Id,
                    JobSkills = new List<JobSkill> { new JobSkill { SkillId = Get("Repair").Id }, new JobSkill { SkillId = Get("AI Programming").Id } } },
            };

            db.Jobs.AddRange(jobs);
            await db.SaveChangesAsync();
        }
    }
}
