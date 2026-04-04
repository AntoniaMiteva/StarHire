using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarHire.Business.Services.Interfaces;
using System.Security.Claims;

namespace StarHire.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly IFavoriteService _favoriteService;
        private readonly IJobService _jobService;

        public FavoritesController(IFavoriteService favoriteService, IJobService jobService)
        {
            _favoriteService = favoriteService;
            _jobService = jobService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var favoriteJobIds = await _favoriteService.GetFavoriteJobIdsAsync(userId);
            var allJobs = await _jobService.GetAll(null, null, null);
            var favoriteJobs = allJobs.Where(j => favoriteJobIds.Contains(j.Id)).ToList();
            return View(favoriteJobs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(Guid jobId, string returnUrl = "/")
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isFav = await _favoriteService.IsFavoriteAsync(jobId, userId);
            if (isFav)
                await _favoriteService.RemoveAsync(jobId, userId);
            else
                await _favoriteService.AddAsync(jobId, userId);

            return Redirect(returnUrl);
        }
    }
}
