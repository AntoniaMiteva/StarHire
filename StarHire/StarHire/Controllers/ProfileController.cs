using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarHire.Business.Services.Interfaces;
using StarHire.Models.ViewModels.Skills;
using System.Security.Claims;

namespace StarHire.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ISkillService _skillService;

        public ProfileController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpGet]
        public async Task<IActionResult> Skills()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var vm = await _skillService.GetProfileSkillsAsync(userId);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Skills(ProfileSkillsViewModel model)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var selected = model.Skills
                .Where(s => s.IsSelected)
                .Select(s => s.Id)
                .ToList();

            await _skillService.UpdateProfileSkillsAsync(userId, selected);

            TempData["SuccessMessage"] = "Skills updated!";
            return RedirectToAction(nameof(Skills));
        }
    }
}
