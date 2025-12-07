using ClubManagement.Service.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClubManagementMVC.Controllers
{
    [Authorize(Roles = "Admin,ClubManager")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            var dashboard = await _dashboardService.GetCompleteDashboardAsync();
            return View(dashboard);
        }

        // GET: Dashboard/Overview
        public async Task<IActionResult> Overview()
        {
            var overview = await _dashboardService.GetOverviewAsync();
            return Json(overview);
        }

        // GET: Dashboard/ClubStats
        public async Task<IActionResult> ClubStats(int topCount = 5)
        {
            var stats = await _dashboardService.GetClubStatisticsAsync(topCount);
            return Json(stats);
        }

        // GET: Dashboard/FinancialStats
        public async Task<IActionResult> FinancialStats()
        {
            var stats = await _dashboardService.GetFinancialStatisticsAsync();
            return Json(stats);
        }

        // GET: Dashboard/ActivityStats
        public async Task<IActionResult> ActivityStats(int upcomingDays = 30)
        {
            var stats = await _dashboardService.GetActivityStatisticsAsync(upcomingDays);
            return Json(stats);
        }

        // GET: Dashboard/MembershipStats
        public async Task<IActionResult> MembershipStats(int trendMonths = 6)
        {
            var stats = await _dashboardService.GetMembershipStatisticsAsync(trendMonths);
            return Json(stats);
        }
    }
}
