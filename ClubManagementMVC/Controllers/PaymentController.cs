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
        [Authorize]
        public async Task<IActionResult> MyPayments()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdString);
            
            // Kiểm tra và cập nhật các payment quá hạn
            await _services.PaymentService.CheckAndUpdateExpiredPaymentsAsync();
            
            var payments = await _services.PaymentService.GetByUserAsync(userId);

            return View(payments);
        }

        // --------------------------
        // 2. Admin/ClubManager xem tất cả
        // --------------------------
        [Authorize(Roles = "Admin,ClubManager")]
        public async Task<IActionResult> Index()
        {
            // Kiểm tra và cập nhật các payment quá hạn
            await _services.PaymentService.CheckAndUpdateExpiredPaymentsAsync();
            
            var allPayments = await _services.PaymentService.GetAllAsync();
            
            // Nếu là ClubManager, chỉ hiển thị payments của clubs mà họ là leader
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _services.UserService.GetByUsernameAsync(username);
                
                if (user == null) return Unauthorized();

                var clubs = await _services.ClubService.GetAllAsync();
                var myClubIds = clubs.Where(c => c.LeaderId == user.UserId).Select(c => c.ClubId).ToList();
                
                // Lấy fees của các clubs mà user là leader
                var allFees = new List<ClubManagement.Service.DTOs.ResponseDTOs.FeeResponseDTO>();
                foreach (var clubId in myClubIds)
                {
                    var fees = await _services.FeeService.GetByClubAsync(clubId);
                    allFees.AddRange(fees);
                }
                var myFeeIds = allFees.Select(f => f.FeeId).ToList();
                
                // Filter payments theo feeIds
                allPayments = allPayments.Where(p => myFeeIds.Contains(p.FeeId));
            }
            
            return View(allPayments);
        }

        // -------------------------
        // 3. Create Payment (Student hoặc ClubManager)
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
   
        [Authorize(Roles = "Admin,ClubManager")]
        [HttpGet]
        public async Task<IActionResult> Confirm(int id)
        {
            var payment = (await _services.PaymentService.GetAllAsync())
                            .FirstOrDefault(p => p.PaymentId == id);

            if (payment == null)
                return NotFound();

            return View(payment);
        }
        [Authorize(Roles = "Admin,ClubManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var payment = await _services.PaymentService.GetByIdAsync(id);

            if (payment == null)
            {
                TempData["error"] = "Không tìm thấy thanh toán này.";
                return RedirectToAction("Index");
            }

            // Kiểm tra quyền: ClubManager chỉ xác nhận payment của clubs mà họ là leader
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _services.UserService.GetByUsernameAsync(username);
                
                if (user == null) return Unauthorized();

                var clubs = await _services.ClubService.GetAllAsync();
                var myClubIds = clubs.Where(c => c.LeaderId == user.UserId).Select(c => c.ClubId).ToList();
                
                // Lấy fee của payment này
                var allFees = new List<ClubManagement.Service.DTOs.ResponseDTOs.FeeResponseDTO>();
                foreach (var clubId in myClubIds)
                {
                    var fees = await _services.FeeService.GetByClubAsync(clubId);
                    allFees.AddRange(fees);
                }
                var myFeeIds = allFees.Select(f => f.FeeId).ToList();
                
                if (!myFeeIds.Contains(payment.FeeId))
                {
                    TempData["error"] = "Bạn không có quyền xác nhận thanh toán này.";
                    return RedirectToAction("Index");
                }
            }

            if (payment.Status != "Pending")
            {
                TempData["error"] = "Chỉ có thể xác nhận các thanh toán đang chờ xác nhận.";
                return RedirectToAction("Index");
            }

            try
            {
                await _services.PaymentService.MarkAsPaidAsync(id);
                TempData["msg"] = "Xác nhận thanh toán thành công!";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            // Kiểm tra xem có referrer từ Fees/Details không
            var referer = Request.Headers["Referer"].ToString();
            if (referer.Contains("/Fees/Details"))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Index");
        }

        // --------------------------
        // Student request đóng phí
        // --------------------------
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestPayment(int paymentId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdString);
            
            // Kiểm tra payment có thuộc về user này không
            var payment = await _services.PaymentService.GetByIdAsync(paymentId);
            if (payment == null || payment.UserId != userId)
            {
                return Forbid();
            }

            try
            {
                await _services.PaymentService.RequestPaymentAsync(paymentId);
                TempData["msg"] = "Đóng phí thành công! Khoản phí đã được xác nhận.";
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return RedirectToAction(nameof(MyPayments));
        }

    }
}
