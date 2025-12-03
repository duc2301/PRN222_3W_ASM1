using AutoMapper;
using ClubManagement.Repository.DbContexts;
using ClubManagement.Repository.Models;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
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
    [Authorize(Roles = "Admin")]
    public class ClubsController : Controller
    {
        private readonly IServiceProviders _serviceProviders;
        private readonly IMapper _mapper;

        public ClubsController(IServiceProviders serviceProviders, IMapper mapper)
        {
            _serviceProviders = serviceProviders;
            _mapper = mapper;
        }





        // GET: Clubs
        public async Task<ActionResult<List<ClubResponseDTO>>> Index()
        {
            var ClubList = await _serviceProviders.ClubService.GetAllAsync();
            return View(ClubList);
        }

        // GET: Clubs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var club = await _serviceProviders.ClubService.GetByIdAsync(id.Value);
            if (club == null)
            {
                return NotFound();
            }

            return View(club);
        }

        // GET: Clubs/Create
        public async Task<IActionResult> Create()
        {
            ViewData["LeaderId"] = new SelectList(await _serviceProviders.UserService.GetAllAsync(), "UserId", "Email");
            return View();
        }

        // POST: Clubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
    }
}
