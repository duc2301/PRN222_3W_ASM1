using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ClubManagementMVC.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IServiceProviders _services;

        public PaymentController(IServiceProviders services)
        {
            _services = services;
        }

        // --------------------------
        // 1. Student xem payment của mình
        // --------------------------
        public async Task<IActionResult> MyPayments()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var payments = await _services.PaymentService.GetByUserAsync(userId);

            return View(payments);
        }

        // --------------------------
        // 2. Admin/Leader xem tất cả
        // --------------------------
        [Authorize(Roles = "Admin,Leader")]
        public async Task<IActionResult> Index()
        {
            var payments = await _services.PaymentService.GetAllAsync();
            return View(payments);
        }

        // -------------------------
        // 3. Create Payment (Student hoặc Leader)
        // -------------------------
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int feeId, decimal amount)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Không cho đóng 2 lần
            if (await _services.PaymentService.HasPaidAsync(userId, feeId))
            {
                ModelState.AddModelError("", "Bạn đã đóng phí này rồi.");
                return View();
            }

            await _services.PaymentService.CreatePaymentAsync(userId, feeId, amount);

            return RedirectToAction(nameof(MyPayments));
        }
   
        [Authorize(Roles = "Admin,Leader")]
        [HttpGet]
        public async Task<IActionResult> Confirm(int id)
        {
            var payment = (await _services.PaymentService.GetAllAsync())
                            .FirstOrDefault(p => p.PaymentId == id);

            if (payment == null)
                return NotFound();

            return View(payment);
        }
        [Authorize(Roles = "Admin,Leader")]
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var payment = await _services.PaymentService.GetByIdAsync(id);

            if (payment == null)
                return NotFound();

            await _services.PaymentService.MarkAsPaidAsync(id);

            TempData["msg"] = "Ghi nhận thanh toán thành công!";
            return RedirectToAction("Index");
        }




    }
}
