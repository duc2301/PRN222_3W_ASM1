using AutoMapper;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ClubManagementMVC.Controllers
{
    public class JoinRequestsController : Controller  // XÓA interface
    {
        private readonly IServiceProviders _serviceProviders;
        private readonly IMapper _mapper;

        public JoinRequestsController(IServiceProviders serviceProviders, IMapper mapper)
        {
            _serviceProviders = serviceProviders;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var requests = await _serviceProviders.JoinRequestService.GetAllAsync();
            return View(requests);
        }

        public async Task<IActionResult> Submit()
        {
            var username = User.Identity?.Name;
            var user = await _serviceProviders.UserService.GetByUsernameAsync(username);

            if (user == null)
                return Unauthorized();

            var dto = new SubmitJoinRequestDTO
            {
                UserId = user.UserId,
                UserEmail = user.Email
            };

            ViewData["ClubId"] = new SelectList(
                await _serviceProviders.ClubService.GetAllAsync(),
                "ClubId",
                "ClubName"
            );

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(SubmitJoinRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["ClubId"] = new SelectList(
                        await _serviceProviders.ClubService.GetAllAsync(),
                        "ClubId",
                        "ClubName"
                    );
                    return View(dto);
                }

                await _serviceProviders.JoinRequestService.SubmitAsync(dto.UserId, dto.ClubId, dto.Note);

                TempData["SuccessMessage"] = "Đã gửi yêu cầu tham gia thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Lỗi logic (đã là member, đã có request pending)
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Submit));
            }
            catch (Exception ex)
            {
                // Lỗi khác
                TempData["ErrorMessage"] = $"Có lỗi xảy ra: {ex.Message}";
                return RedirectToAction(nameof(Submit));
            }
        }

        [Authorize(Roles = "Admin,ClubManager")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                Console.WriteLine($"🎯 CONTROLLER Approve called - RequestId: {id}");

                var username = User.Identity!.Name;
                Console.WriteLine($"👤 Username: {username}");

                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);

                if (user == null)
                {
                    Console.WriteLine($"❌ User not found!");
                    return Unauthorized();
                }

                Console.WriteLine($"✅ User found - UserId: {user.UserId}");
                Console.WriteLine($"📞 Calling JoinRequestService.ApproveAsync...");

                await _serviceProviders.JoinRequestService.ApproveAsync(id, user.UserId);

                Console.WriteLine($"✅✅ Approve SUCCESS!");

                TempData["SuccessMessage"] = "Đã duyệt yêu cầu và thêm thành viên vào câu lạc bộ!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌❌ ERROR in Controller: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Reject(int id)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            var username = User.Identity!.Name;

            if (role != "ClubManager" && role != "Admin")
                return Forbid();

            var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
            if (user == null) return Unauthorized();

            var request = await _serviceProviders.JoinRequestService.GetByIdAsync(id);
            if (request == null) return NotFound();

            var dto = new RejectJoinRequestDTO
            {
                RequestId = id,
                LeaderId = user.UserId
            };

            ViewBag.LeaderId = new SelectList(
                await _serviceProviders.UserService.GetAllAsync(),
                "UserId", "Email", dto.LeaderId
            );

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectConfirmed(RejectJoinRequestDTO dto)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role != "ClubManager" && role != "Admin")
                return Forbid();

            var username = User.Identity!.Name;
            var user = await _serviceProviders.UserService.GetByUsernameAsync(username);

            if (user == null)
                return Unauthorized();

            int leaderId = user.UserId;

            await _serviceProviders.JoinRequestService
                .RejectAsync(dto.RequestId, leaderId, dto.Reason);

            TempData["SuccessMessage"] = "Đã từ chối yêu cầu tham gia!";
            return RedirectToAction(nameof(Index));
        }
    }
}