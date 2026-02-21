using Microsoft.EntityFrameworkCore;
using StarHire.Business.Services.Implementation;
using StarHire.Data;
using StarHire.Models.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Tests
{
    public class SkillServiceTests
    {
        private ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        //GetProfileSkillsAsync

        [Fact]
        public async Task GetProfileSkillsAsync_ReturnsAllSkills()
        {
            await using var db = CreateInMemoryDb();
            db.Skills.AddRange(
                new Skill { Id = Guid.NewGuid(), Name = "C#", NormalizedName = "C#" },
                new Skill { Id = Guid.NewGuid(), Name = "Python", NormalizedName = "PYTHON" }
            );
            await db.SaveChangesAsync();

            var service = new SkillService(db);
            var result = await service.GetProfileSkillsAsync(Guid.NewGuid());

            Assert.Equal(2, result.Skills.Count);
        }

        [Fact]
        public async Task GetProfileSkillsAsync_MarksUserSkillsAsSelected()
        {
            await using var db = CreateInMemoryDb();
            var skillId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            db.Skills.Add(new Skill { Id = skillId, Name = "C#", NormalizedName = "C#" });
            db.UserSkills.Add(new UserSkill { UserId = userId, SkillId = skillId });
            await db.SaveChangesAsync();

            var service = new SkillService(db);
            var result = await service.GetProfileSkillsAsync(userId);

            Assert.True(result.Skills.Single(s => s.Id == skillId).IsSelected);
        }

        [Fact]
        public async Task GetProfileSkillsAsync_SkillsNotOwnedByUser_AreNotSelected()
        {
            await using var db = CreateInMemoryDb();
            var skillId = Guid.NewGuid();

            db.Skills.Add(new Skill { Id = skillId, Name = "Go", NormalizedName = "GO" });
            await db.SaveChangesAsync();

            var service = new SkillService(db);
            var result = await service.GetProfileSkillsAsync(Guid.NewGuid());

            Assert.False(result.Skills.Single().IsSelected);
        }

        [Fact]
        public async Task GetProfileSkillsAsync_ReturnsSkillsOrderedByName()
        {
            await using var db = CreateInMemoryDb();
            db.Skills.AddRange(
                new Skill { Id = Guid.NewGuid(), Name = "Rust", NormalizedName = "RUST" },
                new Skill { Id = Guid.NewGuid(), Name = "Angular", NormalizedName = "ANGULAR" },
                new Skill { Id = Guid.NewGuid(), Name = "Kotlin", NormalizedName = "KOTLIN" }
            );
            await db.SaveChangesAsync();

            var service = new SkillService(db);
            var result = await service.GetProfileSkillsAsync(Guid.NewGuid());

            var names = result.Skills.Select(s => s.Name).ToList();
            Assert.Equal(names.OrderBy(n => n).ToList(), names);
        }

        //UpdateProfileSkillsAsync

        [Fact]
        public async Task UpdateProfileSkillsAsync_AddsNewSkills()
        {
            await using var db = CreateInMemoryDb();
            var userId = Guid.NewGuid();
            var skillId = Guid.NewGuid();

            db.Skills.Add(new Skill { Id = skillId, Name = "C#", NormalizedName = "C#" });
            await db.SaveChangesAsync();

            var service = new SkillService(db);
            await service.UpdateProfileSkillsAsync(userId, new List<Guid> { skillId });

            Assert.Single(db.UserSkills.Where(us => us.UserId == userId));
        }

        [Fact]
        public async Task UpdateProfileSkillsAsync_RemovesOldSkills()
        {
            await using var db = CreateInMemoryDb();
            var userId = Guid.NewGuid();
            var oldSkillId = Guid.NewGuid();
            var newSkillId = Guid.NewGuid();

            db.Skills.AddRange(
                new Skill { Id = oldSkillId, Name = "Old", NormalizedName = "OLD" },
                new Skill { Id = newSkillId, Name = "New", NormalizedName = "NEW" }
            );
            db.UserSkills.Add(new UserSkill { UserId = userId, SkillId = oldSkillId });
            await db.SaveChangesAsync();

            var service = new SkillService(db);
            await service.UpdateProfileSkillsAsync(userId, new List<Guid> { newSkillId });

            Assert.False(db.UserSkills.Any(us => us.SkillId == oldSkillId));
            Assert.True(db.UserSkills.Any(us => us.SkillId == newSkillId));
        }

        [Fact]
        public async Task UpdateProfileSkillsAsync_WithEmptyList_RemovesAllSkills()
        {
            await using var db = CreateInMemoryDb();
            var userId = Guid.NewGuid();
            var skillId = Guid.NewGuid();

            db.Skills.Add(new Skill { Id = skillId, Name = "C#", NormalizedName = "C#" });
            db.UserSkills.Add(new UserSkill { UserId = userId, SkillId = skillId });
            await db.SaveChangesAsync();

            var service = new SkillService(db);
            await service.UpdateProfileSkillsAsync(userId, new List<Guid>());

            Assert.Empty(db.UserSkills.Where(us => us.UserId == userId));
        }

        [Fact]
        public async Task UpdateProfileSkillsAsync_IgnoresDuplicateSkillIds()
        {
            await using var db = CreateInMemoryDb();
            var userId = Guid.NewGuid();
            var skillId = Guid.NewGuid();

            db.Skills.Add(new Skill { Id = skillId, Name = "C#", NormalizedName = "C#" });
            await db.SaveChangesAsync();

            var service = new SkillService(db);
            await service.UpdateProfileSkillsAsync(userId, new List<Guid> { skillId, skillId, skillId });

            Assert.Single(db.UserSkills.Where(us => us.UserId == userId));
        }
    }

}
