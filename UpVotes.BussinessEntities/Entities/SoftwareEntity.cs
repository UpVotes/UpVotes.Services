using System.Collections.Generic;

namespace UpVotes.BusinessEntities.Entities
{
    public class SoftwareEntity
    {
        public int SoftwareID { get; set; }
        public string SoftwareName { get; set; }
        public string LogoName { get; set; }
        public string TagLine { get; set; }
        public int UserRating { get; set; }
        public int NoOfUsersRated { get; set; }
        public int NoOfVotes { get; set; }
        public bool IsVoted { get; set; }
        public string PriceRange { get; set; }
        public string SoftwareTrail { get; set; }
        public string DemoURL { get; set; }
        public string DomainID { get; set; }
        public string WebSiteURL { get; set; }
        public string LinkedInURL { get; set; }
        public string FaceBookURL { get; set; }
        public string TwitterURL { get; set; }
        public string InstagramURL { get; set; }
        public int? FoundedYear { get; set; }
        [System.Web.Mvc.AllowHtml]
        public string SoftwareDescription { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string ServiceCategoryName { get; set; }
        public int ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
        public int ServiceCategoryID { get; set; }
        public bool IsActive { get; set; }
        public int Ranks { get; set; }
        public int TotalCount { get; set; }
        public bool IsClaim { get; set; }

        public string IsUserVerified { get; set; }

        public string UserVerifiedDate { get; set; }

        public string IsAdminApproved { get; set; }

        public string AdminApprovedDate { get; set; }

        public string WorkEmail { get; set; }
        public string Remarks { get; set; }

        public List<SoftwareReviewsEntity> SoftwareReviews { get; set; }
        public List<OverviewNewsResponseEntity> OverviewNewsData { get; set; }
        public int LoggedInUserName { get; set; }

        public string SoftwareCatagoryIds { get; set; }
    }

    public class SoftwareRejectComments
    {
        public int SoftwareId { get; set; }

        public string SoftwareName { get; set; }

        public string RejectComments { get; set; }

        public string RejectedBy { get; set; }

        public  string WorkEmail { get; set; }
    }
}
