using AutoMapper;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
using ClubManagementMVC.Controllers.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using static ClubManagementMVC.Controllers.JoinRequestsController;

namespace ClubManagementMVC.Controllers
{
    
        public class JoinRequestsController : Controller, IJoinRequestController
        {
            private readonly IServiceProviders _serviceProviders;
            private readonly IMapper _mapper;

            public JoinRequestsController(IServiceProviders serviceProviders, IMapper mapper)
            {
                _serviceProviders = serviceProviders;
                _mapper = mapper;
            }

            // ----------------------------------------------------
            // GET: JoinRequests
            // ----------------------------------------------------
            public async Task<IActionResult> Index()
            {
                var requests = await _serviceProviders.JoinRequestService.GetAllAsync();
                return View(requests);
            }


        // ----------------------------------------------------
        // GET: JoinRequests/Submit
        // ----------------------------------------------------
        public async Task<IActionResult> Submit()
        {
            // Lấy username từ Claims (bạn đang lưu ClaimTypes.Name = Username)
            var username = User.Identity?.Name;

            // Lấy user bằng username
            var user = await _serviceProviders.UserService.GetByUsernameAsync(username);

            if (user == null)
                return Unauthorized();

            // Truyền luôn UserId + Email vào DTO
            var dto = new SubmitJoinRequestDTO
            {
                UserId = user.UserId,
                UserEmail = user.Email  // cần thêm vào DTO
            };

            ViewData["ClubId"] = new SelectList(
                await _serviceProviders.ClubService.GetAllAsync(),
                "ClubId",
                "ClubName"
            );

            return View(dto);
        }


        // POST: JoinRequests/Submit
        [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Submit(SubmitJoinRequestDTO dto)
            {
                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                await _serviceProviders.JoinRequestService.SubmitAsync(dto.UserId, dto.ClubId, dto.Note);
                return RedirectToAction(nameof(Index));
            }


        // ----------------------------------------------------
        // GET: JoinRequests/Approve/5
        // ----------------------------------------------------
        [Authorize(Roles = "Admin,ClubManager")]
        public async Task<IActionResult> Approve(int id)
        {
            var username = User.Identity!.Name;

            var user = await _serviceProviders.UserService.GetByUsernameAsync(username);

            if (user == null)
                return Unauthorized();

            await _serviceProviders.JoinRequestService.ApproveAsync(id, user.UserId);

            return RedirectToAction("Index");
        }

        // POST: JoinRequests/ApproveConfirmed
        [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> ApproveConfirmed(ApproveJoinRequestDTO dto)
            {
                await _serviceProviders.JoinRequestService.ApproveAsync(dto.RequestId, dto.LeaderId);
                return RedirectToAction(nameof(Index));
            }


        // ----------------------------------------------------
        // GET: JoinRequests/Reject/5
        // ----------------------------------------------------
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



        // POST: JoinRequests/RejectConfirmed
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

            return RedirectToAction(nameof(Index));
        }


    }
}
