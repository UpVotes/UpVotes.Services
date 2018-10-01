using System.Collections.Generic;
using System.Web;

namespace UpVotes.BusinessEntities.Entities
{
    public class CompanyEntity
    {
        public int CompanyID { get; set; }

        public string CompanyName { get; set; }

        public string LogoName { get; set; }        

        public string TagLine { get; set; }

        public int FoundedYear { get; set; }

        public string URL { get; set; }

        public string LinkedInProfileURL { get; set; }

        public string TwitterProfileURL { get; set; }

        public string FacebookProfileURL { get; set; }

        public string GooglePlusProfileURL { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string Summary { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string Summary1 { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string Summary2 { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string Summary3 { get; set; }

        [System.Web.Mvc.AllowHtml]
        public string KeyClients { get; set; }

        public int Ranks { get; set; }

        public int? RowNumber { get; set; }

        public int TotalCount { get; set; }

        public int UserID { get; set; }

        public string AveragHourlyRate { get; set; }

        public string TotalEmployees { get; set; }

        public int NoOfVotes { get; set; }

        public bool IsVoted { get; set; }

        public int UserRating { get; set; }

        public int NoOfUsersRated { get; set; }

        public int NoOfUsersVoted { get; set; }

        public string BranchName { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public string StateName { get; set; }

        public string CountryName { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string WorkEmail { get; set; }

        public string IsUserVerified { get; set; }

        public string UserVerifiedDate { get; set; }

        public string IsAdminApproved { get; set; }

        public string AdminApprovedDate { get; set; }

        public string Remarks { get; set; }

        public bool IsAdminUser { get; set; }
        public bool IsClaim { get; set; }
        public string CompanyDomain { get; set; }

        public List<CompanyBranchEntity> CompanyBranches { get; set; }

        public List<CompanyFocusEntity> CompanyFocus { get; set; }
        public List<CompanyFocusEntity> IndustialCompanyFocus { get; set; }
        public List<CompanyFocusEntity> CompanyClientFocus { get; set; }
        public List<CompanyFocusEntity> CompanySubFocus { get; set; }
        public List<string> SubfocusNames { get; set; }
        public List<CompanyPortFolioEntity> CompanyPortFolio { get; set; }

        public List<CompanyReviewsEntity> CompanyReviews { get; set; }        
    }

    public class CompanyRejectComments
    {
        public int CompanyID { get; set; }

        public string CompanyName { get; set; }

        public string RejectComments { get; set; }

        public string RejectedBy { get; set; }
    }
}
