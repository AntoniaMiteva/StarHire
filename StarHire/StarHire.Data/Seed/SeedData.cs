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


        //Employer user
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


        // Seed demo jobs
        if (!await db.Jobs.AnyAsync())
        {
            db.Jobs.AddRange(
                new Job { Id = Guid.NewGuid(), Title = "Space Engineer", Planet = "Mars", Salary = 5000, Description = "Maintain Mars stations.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Space Engineer", Planet = "Venus", Salary = 5200, Description = "Station maintenance.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Asteroid Miner", Planet = "Mars", Salary = 7000, Description = "Mining operations.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Asteroid Miner", Planet = "Jupiter", Salary = 7200, Description = "Deep asteroid mining.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "UFO Pilot", Planet = "Saturn", Salary = 9000, Description = "Cargo transportation.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "UFO Pilot", Planet = "Mars", Salary = 8800, Description = "Interplanetary flights.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Galactic Trader", Planet = "Neptune", Salary = 6500, Description = "Trade coordination.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Galactic Trader", Planet = "Venus", Salary = 6700, Description = "Interplanetary trade.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Space Mechanic", Planet = "Mars", Salary = 4800, Description = "Ship repairs.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Space Mechanic", Planet = "Jupiter", Salary = 5100, Description = "Engine maintenance.", EmployeerId = employerUser!.Id },

                new Job { Id = Guid.NewGuid(), Title = "Space Engineer", Planet = "Saturn", Salary = 5400, Description = "Base engineering.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Asteroid Miner", Planet = "Neptune", Salary = 7100, Description = "Mineral extraction.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "UFO Pilot", Planet = "Venus", Salary = 9200, Description = "Luxury space transport.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Galactic Trader", Planet = "Mars", Salary = 6300, Description = "Market expansion.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Space Mechanic", Planet = "Saturn", Salary = 5000, Description = "Station repairs.", EmployeerId = employerUser!.Id },

                new Job { Id = Guid.NewGuid(), Title = "Space Engineer", Planet = "Neptune", Salary = 5600, Description = "Structural design.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Asteroid Miner", Planet = "Saturn", Salary = 7300, Description = "Outer belt mining.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "UFO Pilot", Planet = "Jupiter", Salary = 9100, Description = "Freight pilot.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Galactic Trader", Planet = "Jupiter", Salary = 6900, Description = "Trade logistics.", EmployeerId = employerUser!.Id },
                new Job { Id = Guid.NewGuid(), Title = "Space Mechanic", Planet = "Venus", Salary = 4700, Description = "Equipment diagnostics.", EmployeerId = employerUser!.Id }
            );

            await db.SaveChangesAsync();
        }


    }
}