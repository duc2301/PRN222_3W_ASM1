namespace ClubManagement.Service.DTOs.ResponseDTOs.Dashboard
{
    public class CompleteDashboardDTO
    {
        public DashboardOverviewDTO Overview { get; set; } = new();
        public ClubStatisticsDTO ClubStats { get; set; } = new();
        public FinancialStatisticsDTO FinancialStats { get; set; } = new();
        public ActivityStatisticsDTO ActivityStats { get; set; } = new();
        public MembershipStatisticsDTO MembershipStats { get; set; } = new();
    }
}
