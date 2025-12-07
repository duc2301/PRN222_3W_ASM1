namespace ClubManagement.Service.DTOs.ResponseDTOs.Dashboard
{
    public class MembershipStatisticsDTO
    {
        public Dictionary<string, int> MembershipsByStatus { get; set; } = new();
        public Dictionary<int, int> MonthlyJoinTrends { get; set; } = new();
    }
}
