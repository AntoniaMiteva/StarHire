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

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

       
        public async Task<IActionResult> Index(string? search, string? planet, decimal? minSalary)
        {
            var jobs = await _jobService.GetAll(search, planet, minSalary);

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

        [Authorize(Roles = "Employer")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer")]
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
        public IActionResult MyJobs()
        {
            return View();
        }
    }
}
    