using ClubManagement.Service.DTOs.ResponseDTOs.Dashboard;

namespace ClubManagement.Service.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardOverviewDTO> GetOverviewAsync();
    Task<ClubStatisticsDTO> GetClubStatisticsAsync(int topCount = 5);
    Task<FinancialStatisticsDTO> GetFinancialStatisticsAsync();
    Task<ActivityStatisticsDTO> GetActivityStatisticsAsync(int upcomingDays = 30);
    Task<MembershipStatisticsDTO> GetMembershipStatisticsAsync(int trendMonths = 6);
    Task<CompleteDashboardDTO> GetCompleteDashboardAsync();
}