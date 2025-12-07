namespace ClubManagement.Service.DTOs.ResponseDTOs.Dashboard
{
    public class FinancialStatisticsDTO
    {
        public decimal TotalRevenue { get; set; }
        public int PaidPaymentsCount { get; set; }
        public int PendingPaymentsCount { get; set; }
        public List<UnpaidFeeDTO> UnpaidFees { get; set; } = new();
    }

    public class UnpaidFeeDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
