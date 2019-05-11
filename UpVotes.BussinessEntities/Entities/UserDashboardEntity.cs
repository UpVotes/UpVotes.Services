namespace UpVotes.BusinessEntities.Entities
{
    public class UserDashboardEntity
    {
        public string CompanySoftwareName { get; set; }
        public int? CompanySoftwareID { get; set; }
        public bool IsAdmin { get; set; }
        public bool? IsCompany { get; set; }
        public bool? IsAdminApproved { get; set; }
        public bool? IsUserApproved { get; set; }
        public int UserType { get; set; }
        public bool IsDashboard { get; set; }
        public bool IsService { get; set; }
        public bool IsSoftware { get; set; }
        public bool IsJobListing { get; set; }
        public bool IsEmployees { get; set; }
        public bool IsNews { get; set; }
        public bool IsPortFolio { get; set; }
        public bool IsReview { get; set; }
        public bool IsPricing { get; set; }
        public bool IsBadges { get; set; }
    }
}
