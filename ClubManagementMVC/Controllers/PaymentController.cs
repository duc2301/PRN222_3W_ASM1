using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ClubManagementMVC.Controllers
{
    [Authorize]
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
        [Authorize]
        public IActionResult Index()
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("ClubLeader"))
                return RedirectToAction("Create");

            var list = _services.PaymentService.GetAllAsync();
            return View(list);
        }


        // -------------------------
        // 3. Create Payment (Student hoặc Leader)
        // -------------------------
        [HttpGet]
        public IActionResult Create()
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idString == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = int.Parse(idString);
            string randomSuffix = new Random().Next(1000, 9999).ToString();
            ViewBag.TransferCode = $"nap{userId}";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int feeId, decimal amount)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("UserId claim not found.");

            int userId = int.Parse(userIdClaim);

            

            // Nếu user đã đóng phí (đã có bản ghi Paid) => không cho đóng lại
            if (await _services.PaymentService.HasPaidAsync(userId, feeId))
            {
                ModelState.AddModelError("", "Bạn đã đóng phí này rồi.");
                return View();
            }

            // Tạo payment ở trạng thái Pending (chờ admin duyệt)
            await _services.PaymentService.CreatePaymentAsync(userId, feeId, amount);

            TempData["msg"] = "Yêu cầu thanh toán đã được tạo. Vui lòng chờ Leader/Admin duyệt.";
            return RedirectToAction(nameof(MyPayments));
        }

        // -------------------------
        // 4. Confirm / Approve by Admin/Leader
        // -------------------------
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

        // Shortcut approve direct from list (optional)
        [Authorize(Roles = "Admin,Leader")]
        public async Task<IActionResult> Approve(int id)
        {
            var payment = await _services.PaymentService.GetByIdAsync(id);
            if (payment == null) return NotFound();

            await _services.PaymentService.MarkAsPaidAsync(id);
            TempData["msg"] = "Đã duyệt yêu cầu.";
            return RedirectToAction(nameof(Index));
        }
    }
}
