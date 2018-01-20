using System.Collections.Generic;

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

        public string Summary { get; set; }

        public string Summary1 { get; set; }

        public string Summary2 { get; set; }

        public string Summary3 { get; set; }

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

        public List<CompanyBranchEntity> CompanyBranches { get; set; }

        public List<CompanyFocusEntity> CompanyFocus { get; set; }

        public List<CompanyPortFolioEntity> CompanyPortFolio { get; set; }

        public List<CompanyReviewsEntity> CompanyReviews { get; set; }
    }
}
