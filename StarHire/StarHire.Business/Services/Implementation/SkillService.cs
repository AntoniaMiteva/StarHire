using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StarHire.Business.Services.Interfaces;
using StarHire.Data;
using StarHire.Models.Domain.Entities;
using StarHire.Models.ViewModels.Skills;

namespace StarHire.Business.Services.Implementation
{
    public class SkillService : ISkillService
    {
        private readonly ApplicationDbContext _db;

        public SkillService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ProfileSkillsViewModel> GetProfileSkillsAsync(Guid userId)
        {
            var allSkills = await _db.Skills
                .OrderBy(s => s.Name)
                .ToListAsync();

            var userSkillIds = await _db.UserSkills
                .Where(us => us.UserId == userId)
                .Select(us => us.SkillId)
                .ToListAsync();

            return new ProfileSkillsViewModel
            {
                Skills = allSkills.Select(s => new SkillCheckboxViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsSelected = userSkillIds.Contains(s.Id)
                }).ToList()
            };
        }

        public async Task UpdateProfileSkillsAsync(Guid userId, List<Guid> selectedSkillIds)
        {
            //delete old ones
            var existing = await _db.UserSkills
                .Where(us => us.UserId == userId)
                .ToListAsync();

            _db.UserSkills.RemoveRange(existing);

            //add new ones
            var newOnes = selectedSkillIds
                .Distinct()
                .Select(skillId => new UserSkill
                {
                    UserId = userId,
                    SkillId = skillId
                });

            await _db.UserSkills.AddRangeAsync(newOnes);
            await _db.SaveChangesAsync();
        }
    }
}
