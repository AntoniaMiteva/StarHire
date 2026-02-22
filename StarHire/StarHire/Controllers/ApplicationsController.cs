using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarHire.Business.Services.Interfaces;
using StarHire.Models;
using System.Security.Claims;

namespace StarHire.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationService _applicationService;

        public ApplicationsController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(Guid jobId, string? message)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _applicationService.ApplyAsync(jobId, userId, message ?? string.Empty);

                TempData["SuccessMessage"] = "You have successfully applied for this position! :) ";
                return RedirectToAction("Details", "Jobs", new { id = jobId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", "Jobs", new { id = jobId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while applying :( ";
                return RedirectToAction("Details", "Jobs", new { id = jobId });
            }
        }

        public async Task<IActionResult> MyApplications()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var applications = await _applicationService.GetMyApplicationsAsync(userId);

            return View(applications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employer,Admin")]
        public async Task<IActionResult> UpdateStatus(Guid applicationId, ApplicationStatus status, Guid jobId)
        {
            await _applicationService.UpdateStatusAsync(applicationId, status);
            return RedirectToAction("Applicants", "Jobs", new { jobId = jobId });
        }
    }
}
