using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StarHire.Controllers
{
    [Authorize(Roles = "Admin,Employer")]
    public class JobsController : Controller
    {
        public IActionResult MyJobs()
        {
            return View();
        }
    }
}
    