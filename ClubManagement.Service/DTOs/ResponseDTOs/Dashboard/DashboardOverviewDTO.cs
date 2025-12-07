namespace ClubManagement.Service.DTOs.ResponseDTOs.Dashboard
{
    public class DashboardOverviewDTO
    {
        public int TotalClubs { get; set; }
        public int TotalMembers { get; set; }
        public int TotalActivities { get; set; }
        public int PendingJoinRequests { get; set; }
    }
}
