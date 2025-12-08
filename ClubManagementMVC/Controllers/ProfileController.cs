using AutoMapper;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClubManagementMVC.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IServiceProviders _serviceProviders;
        private readonly IMapper _mapper;

        public ProfileController(IServiceProviders serviceProviders, IMapper mapper)
        {
            _serviceProviders = serviceProviders;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userName = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return RedirectToAction("Login", "Auth");

            var user = await _serviceProviders.UserService.GetByUsernameAsync(userName);

            // Lấy số lượng fee để hiện badge thông báo bên sidebar (nếu muốn)
            var fees = await _serviceProviders.FeeService.GetAvailableFeesAsync(userName);
            ViewBag.FeeCount = fees.Count();

            return View(_mapper.Map<UserResponseDTO>(user));
        }

        [HttpGet]
        public async Task<IActionResult> ClubFeeAvailable()
        {
            var userName = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return RedirectToAction("Login", "Auth");

            var availableFees = await _serviceProviders.FeeService.GetAvailableFeesAsync(userName);

            var user = await _serviceProviders.UserService.GetByUsernameAsync(userName);
            ViewBag.CurrentUser = _mapper.Map<UserResponseDTO>(user);
            ViewBag.FeeCount = availableFees.Count();

            return View(_mapper.Map<List<FeeResponseDTO>>(availableFees));
        }

        [HttpGet]
        public async Task<ActionResult<List<ClubResponseDTO>>> MyClub()
        {
            var userName = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return RedirectToAction("Login", "Auth");

            var clubs = await _serviceProviders.ClubService.GetClubsByUsernameAsync(userName);
            return View(_mapper.Map<List<ClubResponseDTO>>(clubs));

        }

        [HttpGet]
        public async Task<IActionResult> MyTransaction()
        {
            var userName = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(userName)) return RedirectToAction("Login", "Auth");

            var myTrasactions = await _serviceProviders.PaymentService.GetPaymentsByUsernameAsync(userName);

            return View(_mapper.Map<List<PaymentResponseDTO>>(myTrasactions));
        }

    }
}