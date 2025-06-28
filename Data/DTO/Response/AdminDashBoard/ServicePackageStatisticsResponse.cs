namespace TourManagement_BE.Data.DTO.Response.AdminDashBoard
{
    public class ServicePackageStatisticsResponse
    {
        public string Month { get; set; } = string.Empty;
        public int PurchaseCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

}
