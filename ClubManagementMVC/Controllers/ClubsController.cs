using AutoMapper;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
using ClubManagement.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ClubManagementMVC.Controllers
{
    public class ClubsController : Controller
    {
        private readonly IServiceProviders _serviceProviders;
        private readonly IMapper _mapper;
        private readonly IMembershipService _membershipService;

        public ClubsController(
            IServiceProviders serviceProviders,
            IMapper mapper,
            IMembershipService membershipService)
        {
            _serviceProviders = serviceProviders;
            _mapper = mapper;
            _membershipService = membershipService;
        }

        // GET: Clubs
        // ClubManager: chỉ thấy các CLB mình quản lý
        [Authorize(Roles = "Admin,ClubManager")]
        public async Task<IActionResult> Index()
        {
            var clubList = await _serviceProviders.ClubService.GetAllAsync();
            var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;

            if (User.IsInRole("ClubManager"))
            {
                // ClubManager: chỉ thấy CLB của mình
                clubList = clubList
                    .Where(c => c.Leader != null && c.Leader.Username == currentUsername)
                    .OrderBy(c => c.ClubName)
                    .ToList();
            }
            return View(clubList);
        }

        // GET: Clubs/Details/5 + search + phân trang
        [Authorize(Roles = "Admin,ClubManager")]
        public async Task<IActionResult> Details(
            int id,
            string? search,
            string? roleFilter,
            string? statusFilter,
            int page = 1)
        {
            const int pageSize = 10;

            var viewModel = await _serviceProviders.ClubService
                .GetMembersPageAsync(id, search, roleFilter, statusFilter, page, pageSize);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // GET: Clubs/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var leaders = await _serviceProviders.UserService.GetLeadersAsync();
            if (!leaders.Any())
            {
                ViewBag.LeaderError = "No active leaders";
            }

            ViewData["LeaderId"] = new SelectList(leaders, "UserId", "Email");
            return View();
        }

        // POST: Clubs/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ClubId,ClubName,Description,CreatedAt,LeaderId")] CreateClubRequestDTO club)
        {
            if (ModelState.IsValid)
            {
                await _serviceProviders.ClubService.CreateAsync(club);
                return RedirectToAction(nameof(Index));
            }

            ViewData["LeaderId"] = new SelectList(
                await _serviceProviders.UserService.GetLeadersAsync(),
                "UserId",
                "Email",
                club.LeaderId);

            return View(club);
        }

        // GET: Clubs/Edit/5
        [Authorize(Roles = "Admin,ClubManager")]
        public async Task<IActionResult> Edit(int id)
        {
            var club = await _serviceProviders.ClubService.GetByIdAsync(id);
            if (club == null)
            {
                return NotFound();
            }

            // ClubManager chỉ được sửa CLB của chính mình
            if (User.IsInRole("ClubManager"))
            {
                var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
                if (club.Leader == null || club.Leader.Username != currentUsername)
                {
                    ViewBag.LeaderError = "You are not the owner of this club and cannot edit it.";

                    var deniedModel = _mapper.Map<UpdateClubRequestDTO>(club);
                    var leadersDenied = await _serviceProviders.UserService.GetLeadersAsync();
                    ViewData["LeaderId"] = new SelectList(leadersDenied, "UserId", "Email", club.LeaderId);
                    return View(deniedModel);
                }
            }

            var updateRequest = _mapper.Map<UpdateClubRequestDTO>(club);
            var leaders = await _serviceProviders.UserService.GetLeadersAsync();
            if (!leaders.Any())
            {
                ViewBag.LeaderError = "No active leaders";
            }

            ViewData["LeaderId"] = new SelectList(leaders, "UserId", "Email", club.LeaderId);
            return View(updateRequest);
        }

        // POST: Clubs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ClubManager")]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ClubId,ClubName,Description,CreatedAt,LeaderId")] UpdateClubRequestDTO club)
        {
            if (id != club.ClubId)
            {
                return NotFound();
            }

            var clubEntity = await _serviceProviders.ClubService.GetByIdAsync(id);
            if (clubEntity == null)
            {
                return NotFound();
            }

            if (User.IsInRole("ClubManager"))
            {
                var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;
                if (clubEntity.Leader == null || clubEntity.Leader.Username != currentUsername)
                {
                    ViewBag.LeaderError = "You are not the owner of this club and cannot edit it.";
                    ViewData["LeaderId"] = new SelectList(
                        await _serviceProviders.UserService.GetLeadersAsync(),
                        "UserId",
                        "Email",
                        club.LeaderId);
                    return View(club);
                }

                // ClubManager không được đổi Leader
                club.LeaderId = clubEntity.Leader.UserId;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _serviceProviders.ClubService.UpdateAsync(club);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClubExists(club.ClubId))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["LeaderId"] = new SelectList(
                await _serviceProviders.UserService.GetLeadersAsync(),
                "UserId",
                "Email",
                club.LeaderId);

            return View(club);
        }

        // GET: Clubs/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var club = await _serviceProviders.ClubService.GetByIdAsync(id);
            if (club == null)
            {
                return NotFound();
            }

            return View(club);
        }

        // POST: Clubs/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var club = await _serviceProviders.ClubService.GetByIdAsync(id);
            if (club != null)
            {
                await _serviceProviders.ClubService.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClubExists(int id)
        {
            var club = _serviceProviders.ClubService.GetByIdAsync(id).Result;
            return club != null;
        }

        [Authorize(Roles = "Admin,ClubManager")]
        public async Task<IActionResult> MyClubs()
        {
            var allUsers = await _serviceProviders.UserService.GetAllAsync();
            var current = allUsers.FirstOrDefault(u => u.Username == User.Identity!.Name);

            if (current == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var allClubs = await _serviceProviders.ClubService.GetAllAsync();
            var myClubs = allClubs.Where(c => c.LeaderId == current.UserId).ToList();

            return View(myClubs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ClubManager")]
        public async Task<IActionResult> UpdateMember(UpdateMemberRequestDTO model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", new { id = model.ClubId });
            }

            await _membershipService.UpdateMemberAsync(model);
            return RedirectToAction("Details", new { id = model.ClubId });
        }
    }
}
