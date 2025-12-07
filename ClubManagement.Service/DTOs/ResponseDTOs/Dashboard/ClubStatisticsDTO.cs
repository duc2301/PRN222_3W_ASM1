namespace ClubManagement.Service.DTOs.ResponseDTOs.Dashboard
{
    public class ClubStatisticsDTO
    {
        public Dictionary<string, int> TopClubsByMembers { get; set; } = new();
        public Dictionary<string, int> ActivitiesByClub { get; set; } = new();
    }
}
