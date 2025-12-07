using ClubManagement.Service.DTOs.RequestDTOs;
using ClubManagement.Service.ServiceProviders.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ClubManagementMVC.Controllers
{
    [Authorize(Roles = "Admin,ClubManager")]
    public class FeesController : Controller
    {
        private readonly IServiceProviders _serviceProviders;

        public FeesController(IServiceProviders serviceProviders)
        {
            _serviceProviders = serviceProviders;
        }

        // GET: Fees
        public async Task<IActionResult> Index(int? clubId, int page = 1)
        {
            const int pageSize = 5; // Mỗi trang hiển thị 5 phí
            
            IEnumerable<ClubManagement.Service.DTOs.ResponseDTOs.FeeResponseDTO> allFees;

            if (User.IsInRole("ClubManager"))
            {
                // ClubManager chỉ xem fees của clubs mà họ là leader
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                
                if (user == null) return Unauthorized();

                var clubs = await _serviceProviders.ClubService.GetAllAsync();
                var myClubs = clubs.Where(c => c.LeaderId == user.UserId);
                
                var feesList = new List<ClubManagement.Service.DTOs.ResponseDTOs.FeeResponseDTO>();
                foreach (var club in myClubs)
                {
                    var clubFees = await _serviceProviders.FeeService.GetByClubAsync(club.ClubId);
                    feesList.AddRange(clubFees);
                }
                allFees = feesList;
            }
            else
            {
                // Admin xem tất cả hoặc filter theo clubId
                if (clubId.HasValue)
                {
                    allFees = await _serviceProviders.FeeService.GetByClubAsync(clubId.Value);
                }
                else
                {
                    allFees = await _serviceProviders.FeeService.GetAllAsync();
                }
            }

            // Tính toán phân trang
            var totalItems = allFees.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            // Đảm bảo page hợp lệ
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;
            
            // Lấy fees cho trang hiện tại
            var fees = allFees
                .OrderByDescending(f => f.CreatedAt ?? DateTime.MinValue)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Truyền thông tin phân trang qua ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;
            ViewBag.ClubId = clubId; // Giữ lại filter khi chuyển trang

            // Load clubs cho dropdown filter (Admin only)
            if (User.IsInRole("Admin"))
            {
                var clubs = await _serviceProviders.ClubService.GetAllAsync();
                ViewBag.Clubs = new SelectList(clubs, "ClubId", "ClubName", clubId);
            }

            return View(fees);
        }

        // GET: Fees/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Kiểm tra và cập nhật các payment quá hạn
            await _serviceProviders.PaymentService.CheckAndUpdateExpiredPaymentsAsync();
            
            var fee = await _serviceProviders.FeeService.GetByIdAsync(id);
            if (fee == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền: ClubManager chỉ xem fees của clubs mà họ là leader
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                var clubs = await _serviceProviders.ClubService.GetAllAsync();
                var myClub = clubs.FirstOrDefault(c => c.ClubId == fee.ClubId && c.LeaderId == user.UserId);
                
                if (myClub == null)
                {
                    return Forbid();
                }
            }

            // Lấy danh sách payments cho fee này với đầy đủ thông tin User và Fee
            var allPayments = await _serviceProviders.PaymentService.GetAllAsync();
            var payments = allPayments
                .Where(p => p.FeeId == id)
                .OrderByDescending(p => p.PaymentDate)
                .ThenBy(p => p.Status == "Pending" ? 0 : (p.Status == "Unpaid" ? 1 : (p.Status == "Paid" ? 2 : 3)))
                .ToList();
            ViewBag.Payments = payments;

            return View(fee);
        }

        // GET: Fees/Create
        public async Task<IActionResult> Create(int? clubId)
        {
            IEnumerable<ClubManagement.Service.DTOs.ResponseDTOs.ClubResponseDTO> clubs;

            if (User.IsInRole("ClubManager"))
            {
                // ClubManager chỉ tạo fee cho clubs mà họ là leader
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                
                if (user == null) return Unauthorized();

                var allClubs = await _serviceProviders.ClubService.GetAllAsync();
                clubs = allClubs.Where(c => c.LeaderId == user.UserId);
            }
            else
            {
                // Admin có thể tạo fee cho bất kỳ club nào
                clubs = await _serviceProviders.ClubService.GetAllAsync();
            }

            ViewData["ClubId"] = new SelectList(clubs, "ClubId", "ClubName", clubId);
            
            // Nếu có clubId từ route, tạo DTO với clubId đã chọn
            if (clubId.HasValue)
            {
                var dto = new CreateFeeRequestDTO { ClubId = clubId.Value };
                return View(dto);
            }
            
            return View();
        }

        // POST: Fees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateFeeRequestDTO dto)
        {
            // Validate DueDate phải lớn hơn ngày hiện tại
            if (dto.DueDate <= DateOnly.FromDateTime(DateTime.Now))
            {
                ModelState.AddModelError("DueDate", "Hạn nộp phải lớn hơn ngày hiện tại.");
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra quyền: ClubManager chỉ tạo fee cho clubs mà họ là leader
                if (User.IsInRole("ClubManager"))
                {
                    var username = User.Identity?.Name;
                    var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                    var clubs = await _serviceProviders.ClubService.GetAllAsync();
                    var myClub = clubs.FirstOrDefault(c => c.ClubId == dto.ClubId && c.LeaderId == user.UserId);
                    
                    if (myClub == null)
                    {
                        ModelState.AddModelError("", "Bạn không có quyền tạo phí cho câu lạc bộ này.");
                        var allClubs = await _serviceProviders.ClubService.GetAllAsync();
                        ViewData["ClubId"] = new SelectList(allClubs.Where(c => c.LeaderId == user.UserId), "ClubId", "ClubName", dto.ClubId);
                        return View(dto);
                    }
                }

                await _serviceProviders.FeeService.CreateAsync(dto);
                TempData["msg"] = "Tạo phí thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Reload clubs dropdown
            IEnumerable<ClubManagement.Service.DTOs.ResponseDTOs.ClubResponseDTO> clubsList;
            if (User.IsInRole("ClubManager"))
            {
                var username = User.Identity?.Name;
                var user = await _serviceProviders.UserService.GetByUsernameAsync(username);
                var allClubs = await _serviceProviders.ClubService.GetAllAsync();
                clubsList = allClubs.Where(c => c.LeaderId == user.UserId);
            }
            else
            {
                clubsList = await _serviceProviders.ClubService.GetAllAsync();
            }
            ViewData["ClubId"] = new SelectList(clubsList, "ClubId", "ClubName", dto.ClubId);
            return View(dto);
        }
    }
}

