using System.Collections.Generic;

namespace UpVotes.BusinessEntities.Entities
{
    //[DataContract]
    //public class ServiceRequest<T>
    //{
    //   public T RequestObject { get; set; }
    //}    
    public class ClaimApproveRejectListingRequest
    {
        public int ClaimListingID { get; set; }
        public int companyID { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Domain { get; set; }
        public int userID { get; set; }
        public bool IsUserVerify { get; set; }
        public bool IsAdminApproved { get; set; }
        public string RejectionComment { get; set; }
    }

    public class ClaimInfoDetail
    {
        public int ClaimListingID { get; set; }
        public int CompanyID { get; set; }
        public string CompanyDomain { get; set; }
        public string CompanyName { get; set; }
        public string IsUserApproved { get; set; }
        public string URL { get; set; }
        public string WorkEmail { get; set; }
        public string ProfileURL { get; set; }
        public string UpvotesURL { get; set; }
        public string UserApprovedDate { get; set; }
    }

    public class ServiceResponseClaim<T>
    {
        
        public T ResponseObject { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseDescription { get; set; }
    }
}
