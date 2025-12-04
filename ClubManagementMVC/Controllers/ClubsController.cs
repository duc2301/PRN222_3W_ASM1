using AutoMapper;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Models;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
using ClubManagement.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClubManagementMVC.Controllers
{
    public class ClubsController : Controller
    {
        private readonly IServiceProviders _serviceProviders;
        private readonly IMapper _mapper;
        private readonly IMembershipService _membershipService;
        public ClubsController(IServiceProviders serviceProviders, IMapper mapper, IMembershipService membershipService)
        {
            _serviceProviders = serviceProviders;
            _mapper = mapper;
            _membershipService = membershipService;
        }

        // GET: Clubs
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ClubResponseDTO>>> Index()
        {
            var ClubList = await _serviceProviders.ClubService.GetAllAsync();
            return View(ClubList);
        }

        // GET: Clubs/Details/5 + search + phân trang
        [Authorize(Roles = "Admin,ClubManager")]
        public async Task<ActionResult> Details(int id, string? search, string? roleFilter, string? statusFilter, int page = 1)
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
            ViewData["LeaderId"] = new SelectList(await _serviceProviders.UserService.GetAllAsync(), "UserId", "Email");
            return View();
        }

        // POST: Clubs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ClubId,ClubName,Description,CreatedAt,LeaderId")] CreateClubRequestDTO club)
        {
            if (ModelState.IsValid)
            {
                await _serviceProviders.ClubService.CreateAsync(club);
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeaderId"] = new SelectList(await _serviceProviders.UserService.GetAllAsync(), "UserId", "Email", club.LeaderId);
            return View(club);
        }

        // GET: Clubs/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var club = await _serviceProviders.ClubService.GetByIdAsync(id);
            if (club == null)
            {
                return NotFound();
            }

            var updateRequest = _mapper.Map<UpdateClubRequestDTO>(club);

            ViewData["LeaderId"] = new SelectList(await _serviceProviders.UserService.GetAllAsync(), "UserId", "Email", club.LeaderId);
            return View(updateRequest);
        }

        // POST: Clubs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ClubManager")]
        public async Task<IActionResult> Edit(int id, [Bind("ClubId,ClubName,Description,CreatedAt,LeaderId")] UpdateClubRequestDTO club)
        {
            if (id != club.ClubId)
            {
                return NotFound();
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
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeaderId"] = new SelectList(await _serviceProviders.UserService.GetAllAsync(), "UserId", "Email", club.LeaderId);
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
            var current = allUsers.FirstOrDefault(u => u.Username == User.Identity.Name);

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
                // Nếu muốn hiển thị lỗi đẹp hơn có thể load lại Details,
                // tạm thời redirect thẳng cho đơn giản.
                return RedirectToAction("Details", new { id = model.ClubId });
            }

            await _membershipService.UpdateMemberAsync(model);

            // Quay lại trang chi tiết CLB, tab Thành viên
            return RedirectToAction("Details", new { id = model.ClubId });
        }

    }
}
