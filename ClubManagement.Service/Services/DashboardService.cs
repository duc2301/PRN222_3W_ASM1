using ClubManagement.Repository.Repositories.Interfaces;
using ClubManagement.Service.DTOs.ResponseDTOs.Dashboard;
using ClubManagement.Service.Services.Interfaces;

namespace ClubManagement.Service.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardOverviewDTO> GetOverviewAsync()
    {
        return new DashboardOverviewDTO
        {
            TotalClubs = await _dashboardRepository.GetTotalClubsAsync(),
            TotalMembers = await _dashboardRepository.GetTotalMembersAsync(),
            TotalActivities = await _dashboardRepository.GetTotalActivitiesAsync(),
            PendingJoinRequests = await _dashboardRepository.GetPendingJoinRequestsAsync()
        };
    }

    public async Task<ClubStatisticsDTO> GetClubStatisticsAsync(int topCount = 5)
    {
        return new ClubStatisticsDTO
        {
            TopClubsByMembers = await _dashboardRepository.GetTopClubsByMembersAsync(topCount),
            ActivitiesByClub = await _dashboardRepository.GetActivitiesByClubAsync()
        };
    }

    public async Task<FinancialStatisticsDTO> GetFinancialStatisticsAsync()
    {
        var unpaidFees = await _dashboardRepository.GetUnpaidFeesAsync();
        
        return new FinancialStatisticsDTO
        {
            TotalRevenue = await _dashboardRepository.GetTotalRevenueAsync(),
            PaidPaymentsCount = await _dashboardRepository.GetPaidPaymentsCountAsync(),
            PendingPaymentsCount = await _dashboardRepository.GetPendingPaymentsCountAsync(),
            UnpaidFees = unpaidFees.Select(u => new UnpaidFeeDTO
            {
                FullName = u.FullName,
                ClubName = u.ClubName,
                Amount = u.Amount
            }).ToList()
        };
    }

    public async Task<ActivityStatisticsDTO> GetActivityStatisticsAsync(int upcomingDays = 30)
    {
        var upcomingActivities = await _dashboardRepository.GetUpcomingActivitiesAsync(upcomingDays);
        
        return new ActivityStatisticsDTO
        {
            UpcomingActivities = upcomingActivities.Select(a => new UpcomingActivityDTO
            {
                ActivityName = a.ActivityName,
                StartDate = a.StartDate,
                ClubName = a.ClubName
            }).ToList(),
            ParticipationStats = await _dashboardRepository.GetActivityParticipationStatsAsync()
        };
    }

    public async Task<MembershipStatisticsDTO> GetMembershipStatisticsAsync(int trendMonths = 6)
    {
        return new MembershipStatisticsDTO
        {
            MembershipsByStatus = await _dashboardRepository.GetMembershipsByStatusAsync(),
            MonthlyJoinTrends = await _dashboardRepository.GetMonthlyJoinTrendsAsync(trendMonths)
        };
    }

    public async Task<CompleteDashboardDTO> GetCompleteDashboardAsync()
    {
        return new CompleteDashboardDTO
        {
            Overview = await GetOverviewAsync(),
            ClubStats = await GetClubStatisticsAsync(),
            FinancialStats = await GetFinancialStatisticsAsync(),
            ActivityStats = await GetActivityStatisticsAsync(),
            MembershipStats = await GetMembershipStatisticsAsync()
        };
    }
}