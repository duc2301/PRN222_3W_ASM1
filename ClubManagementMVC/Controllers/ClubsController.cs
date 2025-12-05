using AutoMapper;
using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClubManagementMVC.Controllers
{
    [Authorize(Roles = "Admin,ClubManager")]
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
            var clubList = await _serviceProviders.ClubService.GetAllAsync();

            var currentUsername = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            // If ClubManager, only show clubs where they are the leader
            if (User.IsInRole("ClubManager"))
            {
                clubList = clubList
                    .Where(c => c.Leader != null && c.Leader.Username == currentUsername)
                    .OrderBy(c => c.ClubName)
                    .ToList();
            }
            else
            {
                // Admin: sort with their own clubs first, then others
                clubList = clubList
                    .OrderByDescending(c => c.Leader != null && c.Leader.Username == currentUsername)
                    .ThenBy(c => c.ClubName)
                    .ToList();
            }

            return View(clubList);
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
            var leaders = await _serviceProviders.UserService.GetLeadersAsync();
            if (!leaders.Any())
            {
                ViewBag.LeaderError = "No active leaders";
            }
            ViewData["LeaderId"] = new SelectList(leaders, "UserId", "Email");
            return View();
        }

        // POST: Clubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClubId,ClubName,Description,CreatedAt,LeaderId")] CreateClubRequestDTO club)
        {
            if (ModelState.IsValid)
            {
                await _serviceProviders.ClubService.CreateAsync(club);
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeaderId"] = new SelectList(await _serviceProviders.UserService.GetLeadersAsync(), "UserId", "Email", club.LeaderId);
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

            // Only allow ClubManager to edit their own club
            if (User.IsInRole("ClubManager"))
            {
                var currentUsername = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                if (club.Leader == null || club.Leader.Username != currentUsername)
                {
                    ViewBag.LeaderError = "You are not the owner of this club and cannot edit it.";
                    var updateRequest = _mapper.Map<UpdateClubRequestDTO>(club);
                    var leaders = await _serviceProviders.UserService.GetLeadersAsync();
                    ViewData["LeaderId"] = new SelectList(leaders, "UserId", "Email", club.LeaderId);
                    return View(updateRequest);
                }
            }

            var updateRequestValid = _mapper.Map<UpdateClubRequestDTO>(club);
            var leadersValid = await _serviceProviders.UserService.GetLeadersAsync();
            if (!leadersValid.Any())
            {
                ViewBag.LeaderError = "No active leaders";
            }
            ViewData["LeaderId"] = new SelectList(leadersValid, "UserId", "Email", club.LeaderId);
            return View(updateRequestValid);
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

            var clubEntity = await _serviceProviders.ClubService.GetByIdAsync(id);
            if (User.IsInRole("ClubManager"))
            {
                var currentUsername = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
                if (clubEntity == null || clubEntity.Leader == null || clubEntity.Leader.Username != currentUsername)
                {
                    ViewBag.LeaderError = "You are not the owner of this club and cannot edit it.";
                    ViewData["LeaderId"] = new SelectList(await _serviceProviders.UserService.GetLeadersAsync(), "UserId", "Email", club.LeaderId);
                    return View(club);
                }
                // Prevent ClubManager from changing LeaderId
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
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeaderId"] = new SelectList(await _serviceProviders.UserService.GetLeadersAsync(), "UserId", "Email", club.LeaderId);
            return View(club);
        }

        // GET: Clubs/Delete/5
        [Authorize(Roles = "Admin")]
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
    }
}
