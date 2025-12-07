using ClubManagement.Service.DTOs.RequestDTOs.Activity;
using ClubManagement.Service.DTOs.ResponseDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
using ClubManagement.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

namespace ClubManagementMVC.Controllers
{
    [Authorize(Roles = "Admin,ClubManager")]
    public class ActivitiesController : Controller
    {
        private readonly IActivityService _activityService;
        private readonly IClubService _clubService;
        private readonly IServiceProviders _serviceProviders;

        public ActivitiesController(IActivityService activityService, IClubService clubService, IServiceProviders serviceProviders)
        {
            _activityService = activityService;
            _clubService = clubService;
            _serviceProviders = serviceProviders;
        }

        // GET: Activities
        public async Task<IActionResult> Index()
        {
            var allActivities = await _activityService.GetAllAsync();
            
            // Nếu là ClubManager, chỉ hiển thị activities của clubs mà họ là leader
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                
                if (user == null) return Unauthorized();

                var clubs = await _clubService.GetAllAsync();
                var myClubIds = clubs.Where(c => c.LeaderId == user.UserId).Select(c => c.ClubId).ToList();
                
                // Filter activities theo clubIds
                allActivities = allActivities.Where(a => myClubIds.Contains(a.ClubId)).ToList();
            }
            
            return View(allActivities);
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

            // Kiểm tra quyền: ClubManager chỉ xem activities của clubs mà họ là leader
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                var clubs = await _clubService.GetAllAsync();
                var myClub = clubs.FirstOrDefault(c => c.ClubId == activity.ClubId && c.LeaderId == user.UserId);
                
                if (myClub == null)
                {
                    return Forbid();
                }
            }

            return View(activity);
        }

        // GET: Activities/Create
        public async Task<IActionResult> Create()
        {
            IEnumerable<ClubResponseDTO> clubs;
            
            if (User.IsInRole("ClubManager"))
            {
                // ClubManager chỉ tạo activity cho clubs mà họ là leader
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                
                if (user == null) return Unauthorized();

                var allClubs = await _clubService.GetAllAsync();
                clubs = allClubs.Where(c => c.LeaderId == user.UserId);
            }
            else
            {
                // Admin có thể tạo activity cho bất kỳ club nào
                clubs = await _clubService.GetAllAsync();
            }
            
            ViewData["ClubId"] = new SelectList(clubs, "ClubId", "ClubName");
            return View();
        }

        // POST: Activities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClubId,ActivityName,Description,StartDate,EndDate,Location")] ActivityCreateDTO activity)
        {
            // Kiểm tra quyền: ClubManager chỉ tạo activity cho clubs mà họ là leader
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                var clubs = await _clubService.GetAllAsync();
                var myClub = clubs.FirstOrDefault(c => c.ClubId == activity.ClubId && c.LeaderId == user.UserId);
                
                if (myClub == null)
                {
                    ModelState.AddModelError("", "Bạn không có quyền tạo hoạt động cho câu lạc bộ này.");
                    var allClubs = await _clubService.GetAllAsync();
                    ViewData["ClubId"] = new SelectList(allClubs.Where(c => c.LeaderId == user.UserId), "ClubId", "ClubName", activity.ClubId);
                    return View(activity);
                }
            }
            
            if (ModelState.IsValid)
            {
                await _activityService.CreateAsync(activity);
                return RedirectToAction(nameof(Index));
            }
            
            // Reload clubs dropdown
            IEnumerable<ClubResponseDTO> clubsList;
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                var allClubs = await _clubService.GetAllAsync();
                clubsList = allClubs.Where(c => c.LeaderId == user.UserId);
            }
            else
            {
                clubsList = await _clubService.GetAllAsync();
            }
            ViewData["ClubId"] = new SelectList(clubsList, "ClubId", "ClubName", activity.ClubId);
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

            // Kiểm tra quyền: ClubManager chỉ edit activities của clubs mà họ là leader
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                var clubs = await _clubService.GetAllAsync();
                var myClub = clubs.FirstOrDefault(c => c.ClubId == activity.ClubId && c.LeaderId == user.UserId);
                
                if (myClub == null)
                {
                    return Forbid();
                }
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

            // Load clubs dropdown (ClubManager chỉ thấy clubs của mình)
            IEnumerable<ClubResponseDTO> clubsList;
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                var allClubs = await _clubService.GetAllAsync();
                clubsList = allClubs.Where(c => c.LeaderId == user.UserId);
            }
            else
            {
                clubsList = await _clubService.GetAllAsync();
            }
            
            ViewData["ClubId"] = new SelectList(clubsList, "ClubId", "ClubName", activity.ClubId);
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

            // Kiểm tra quyền: ClubManager chỉ edit activities của clubs mà họ là leader
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                var clubs = await _clubService.GetAllAsync();
                var myClub = clubs.FirstOrDefault(c => c.ClubId == activity.ClubId && c.LeaderId == user.UserId);
                
                if (myClub == null)
                {
                    ModelState.AddModelError("", "Bạn không có quyền chỉnh sửa hoạt động của câu lạc bộ này.");
                    var allClubs = await _clubService.GetAllAsync();
                    ViewData["ClubId"] = new SelectList(allClubs.Where(c => c.LeaderId == user.UserId), "ClubId", "ClubName", activity.ClubId);
                    return View(activity);
                }
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
            
            // Reload clubs dropdown
            IEnumerable<ClubResponseDTO> clubsList;
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                var allClubs = await _clubService.GetAllAsync();
                clubsList = allClubs.Where(c => c.LeaderId == user.UserId);
            }
            else
            {
                clubsList = await _clubService.GetAllAsync();
            }
            ViewData["ClubId"] = new SelectList(clubsList, "ClubId", "ClubName", activity.ClubId);
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

            // Kiểm tra quyền: ClubManager chỉ delete activities của clubs mà họ là leader
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                var clubs = await _clubService.GetAllAsync();
                var myClub = clubs.FirstOrDefault(c => c.ClubId == activity.ClubId && c.LeaderId == user.UserId);
                
                if (myClub == null)
                {
                    return Forbid();
                }
            }

            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Kiểm tra quyền trước khi delete
            var activity = await _activityService.GetByIdAsync(id);
            if (activity != null && User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                var clubs = await _clubService.GetAllAsync();
                var myClub = clubs.FirstOrDefault(c => c.ClubId == activity.ClubId && c.LeaderId == user.UserId);
                
                if (myClub == null)
                {
                    return Forbid();
                }
            }
            
            await _activityService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
