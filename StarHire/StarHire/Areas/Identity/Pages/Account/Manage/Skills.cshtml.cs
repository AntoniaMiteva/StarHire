using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StarHire.Data;
using StarHire.Models.Domain.Entities;

namespace StarHire.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class SkillsModel : PageModel
    {
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly ApplicationDbContext _db;

        public SkillsModel(UserManager<IdentityUser<Guid>> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public List<string> MySkills { get; set; } = new();
        public List<string> AllSkills { get; set; } = new();

        [BindProperty]
        public string SkillName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            await LoadData(user.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var name = (SkillName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["ErrorMessage"] = "Enter a skill name.";
                return RedirectToPage();
            }

            var normalized = name.ToUpperInvariant();

            // 1) Find or create Skill
            var skill = await _db.Skills.FirstOrDefaultAsync(s => s.NormalizedName == normalized);
            if (skill == null)
            {
                skill = new Skill
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    NormalizedName = normalized
                };
                _db.Skills.Add(skill);
                await _db.SaveChangesAsync();
            }

            // 2) Add to user if not already
            var already = await _db.UserSkills.AnyAsync(us => us.UserId == user.Id && us.SkillId == skill.Id);
            if (already)
            {
                TempData["ErrorMessage"] = "You already have this skill.";
                return RedirectToPage();
            }

            _db.UserSkills.Add(new UserSkill { UserId = user.Id, SkillId = skill.Id });
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Skill added.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveAsync(string skillName)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var name = (skillName ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name)) return RedirectToPage();

            var normalized = name.ToUpperInvariant();

            var skill = await _db.Skills.FirstOrDefaultAsync(s => s.NormalizedName == normalized);
            if (skill == null) return RedirectToPage();

            var link = await _db.UserSkills.FirstOrDefaultAsync(us => us.UserId == user.Id && us.SkillId == skill.Id);
            if (link == null) return RedirectToPage();

            _db.UserSkills.Remove(link);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Skill removed.";
            return RedirectToPage();
        }

        private async Task LoadData(Guid userId)
        {
            MySkills = await _db.UserSkills
                .Where(us => us.UserId == userId)
                .Join(_db.Skills, us => us.SkillId, s => s.Id, (us, s) => s.Name)
                .OrderBy(x => x)
                .ToListAsync();

            AllSkills = await _db.Skills
                .Select(s => s.Name)
                .OrderBy(x => x)
                .ToListAsync();
        }
    }
}