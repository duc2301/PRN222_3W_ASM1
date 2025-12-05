using Microsoft.AspNetCore.Mvc;

namespace ClubManagementMVC.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
