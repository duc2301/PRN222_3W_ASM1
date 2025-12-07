namespace ClubManagement.Service.DTOs.ResponseDTOs.Dashboard
{
    public class ActivityStatisticsDTO
    {
        public List<UpcomingActivityDTO> UpcomingActivities { get; set; } = new();
        public Dictionary<string, int> ParticipationStats { get; set; } = new();
    }

    public class UpcomingActivityDTO
    {
        public string ActivityName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string ClubName { get; set; } = string.Empty;
    }
}
