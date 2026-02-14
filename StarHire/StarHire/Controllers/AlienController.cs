using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StarHire.Controllers
{
    public class AlienController : Controller
    {
        [Authorize(Roles = "Alien")]
        public IActionResult Index(int id)
        {
            return View();
        }

    }
}
