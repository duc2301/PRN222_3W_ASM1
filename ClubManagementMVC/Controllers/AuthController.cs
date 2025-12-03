using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClubManagementMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IServiceProviders _serviceProviders;

        public AuthController(IServiceProviders serviceProviders)
        {
            _serviceProviders = serviceProviders;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO loginDto, bool rememberMe = false)
        {
            if (ModelState.IsValid)
            {
                var user = await _serviceProviders.AuthService.Login(loginDto.Username, loginDto.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                    return View(loginDto);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username), 
                    new Claim(ClaimTypes.Role, user.Role) 
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe, 
                    ExpiresUtc = DateTime.UtcNow.AddDays(7) 
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Index", "Home");
            }

            return View(loginDto);
        }

        [HttpGet]         
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}
