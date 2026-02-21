using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StarHire.Data;
using StarHire.Models.Domain.Entities;

namespace StarHire.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index() => View();

        // 1) List page
        [HttpGet]
        public async Task<IActionResult> Skills()
        {
            var skills = await _db.Skills
                .OrderBy(s => s.Name)
                .ToListAsync();

            return View(skills);
        }

        // 2) Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSkill(string name)
        {
            name = (name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Skill name is required.";
                return RedirectToAction(nameof(Skills));
            }

            var normalized = name.ToUpper();

            var exists = await _db.Skills.AnyAsync(s => s.NormalizedName == normalized);
            if (exists)
            {
                TempData["Error"] = "Skill already exists.";
                return RedirectToAction(nameof(Skills));
            }

            _db.Skills.Add(new Skill
            {
                Id = Guid.NewGuid(),
                Name = name,
                NormalizedName = normalized
            });

            await _db.SaveChangesAsync();
            TempData["Success"] = "Skill added.";
            return RedirectToAction(nameof(Skills));
        }

        // 3) Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSkill(Guid id)
        {
            var skill = await _db.Skills.FirstOrDefaultAsync(s => s.Id == id);
            if (skill == null)
            {
                TempData["Error"] = "Skill not found.";
                return RedirectToAction(nameof(Skills));
            }

            // remove links first
            var links = _db.UserSkills.Where(us => us.SkillId == id);
            _db.UserSkills.RemoveRange(links);

            _db.Skills.Remove(skill);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Skill deleted.";
            return RedirectToAction(nameof(Skills));
        }
    }
}