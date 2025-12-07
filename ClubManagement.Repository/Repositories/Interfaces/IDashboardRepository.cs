namespace ClubManagement.Repository.Repositories.Interfaces;

public interface IDashboardRepository
{
    // Overview Statistics
    Task<int> GetTotalClubsAsync();
    Task<int> GetTotalMembersAsync();
    Task<int> GetTotalActivitiesAsync();
    Task<int> GetPendingJoinRequestsAsync();
    
    // Club Statistics
    Task<Dictionary<string, int>> GetTopClubsByMembersAsync(int topCount = 5);
    Task<Dictionary<string, int>> GetActivitiesByClubAsync();
    
    // Financial Statistics
    Task<decimal> GetTotalRevenueAsync();
    Task<int> GetPaidPaymentsCountAsync();
    Task<int> GetPendingPaymentsCountAsync();
    Task<List<(string FullName, string ClubName, decimal Amount)>> GetUnpaidFeesAsync();
    
    // Activity Statistics
    Task<List<(string ActivityName, DateTime StartDate, string ClubName)>> GetUpcomingActivitiesAsync(int days = 30);
    Task<Dictionary<string, int>> GetActivityParticipationStatsAsync();
    
    // Membership Statistics
    Task<Dictionary<string, int>> GetMembershipsByStatusAsync();
    Task<Dictionary<int, int>> GetMonthlyJoinTrendsAsync(int months = 6);
}