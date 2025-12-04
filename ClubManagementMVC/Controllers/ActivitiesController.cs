using ClubManagement.Service.DTOs.RequestDTOs.Activity;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace ClubManagementMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ActivitiesController : Controller
    {
        private readonly IActivityService _activityService;
        private readonly IClubService _clubService;

        public ActivitiesController(IActivityService activityService, IClubService clubService)
        {
            _activityService = activityService;
            _clubService = clubService;
        }

        // GET: Activities
        public async Task<IActionResult> Index()
        {
            var activities = await _activityService.GetAllAsync();
            return View(activities);
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _activityService.GetByIdAsync(id.Value);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // GET: Activities/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ClubId"] = new SelectList(await _clubService.GetAllAsync(), "ClubId", "ClubName");
            return View();
        }

        // POST: Activities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClubId,ActivityName,Description,StartDate,EndDate,Location")] ActivityCreateDTO activity)
        {
            if (ModelState.IsValid)
            {
                await _activityService.CreateAsync(activity);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClubId"] = new SelectList(await _clubService.GetAllAsync(), "ClubId", "ClubName", activity.ClubId);
            return View(activity);
        }

        // GET: Activities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _activityService.GetByIdAsync(id.Value);
            if (activity == null)
            {
                return NotFound();
            }

            var updateDto = new ActivityUpdateDTO
            {
                ActivityId = activity.ActivityId,
                ClubId = activity.ClubId,
                ActivityName = activity.ActivityName,
                Description = activity.Description,
                StartDate = activity.StartDate,
                EndDate = activity.EndDate,
                Location = activity.Location
            };

            ViewData["ClubId"] = new SelectList(await _clubService.GetAllAsync(), "ClubId", "ClubName", activity.ClubId);
            return View(updateDto);
        }

        // POST: Activities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActivityId,ClubId,ActivityName,Description,StartDate,EndDate,Location")] ActivityUpdateDTO activity)
        {
            if (id != activity.ActivityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _activityService.UpdateAsync(activity);
                }
                catch
                {
                    if (await _activityService.GetByIdAsync(id) == null)
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
            ViewData["ClubId"] = new SelectList(await _clubService.GetAllAsync(), "ClubId", "ClubName", activity.ClubId);
            return View(activity);
        }

        // GET: Activities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _activityService.GetByIdAsync(id.Value);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _activityService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
