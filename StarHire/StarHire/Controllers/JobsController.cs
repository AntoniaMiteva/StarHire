using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarHire.Business.Services.Interfaces;
using StarHire.Models.ViewModels.Jobs;
using System.Security.Claims;

namespace StarHire.Controllers
{
    public class JobsController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IFavoriteService _favoriteService;

        public JobsController(IJobService jobService, IFavoriteService favoriteService)
        {
            _jobService = jobService;
            _favoriteService = favoriteService;
        }


       
        public async Task<IActionResult> Index(string? search, string? planet, decimal? minSalary)
        {
            Guid? userId = null;
            if (User.IsInRole("Alien"))
            {
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                ViewBag.FavoriteIds = await _favoriteService.GetFavoriteJobIdsAsync(userId.Value);
            }

            var jobs = await _jobService.GetAll(search, planet, minSalary, userId);
            ViewBag.Search = search;
            ViewBag.Planet = planet;
            ViewBag.MinSalary = minSalary;
            return View(jobs);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var job = await _jobService.GetById(id);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Create(CreateJobViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var employerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _jobService.Create(model, employerId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong while creating the job!");
                return View(model);
            }
        }

        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> MyJobs()
        {
            var employerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var jobs = await _jobService.GetByEmployer(employerId);
            return View(jobs);
        }

        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Applicants(Guid jobId)
        {
            var employerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var applicants = await _jobService.GetApplicants(jobId, employerId);

            if (applicants == null)
                return Forbid();

            ViewBag.JobId = jobId;
            return View(applicants);
        }

        [Authorize(Roles = "Admin,Employer")]
        public async Task<IActionResult> Create()
        {
            var model = new CreateJobViewModel
            {
                Skills = await _jobService.GetAllSkillsAsync()
            };
            return View(model);
        }
    }
}
    