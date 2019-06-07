using System;

namespace UpVotes.BusinessEntities.Entities
{
    public class UserReviewsResponseEntity
    {
        public int ReviewID { get; set; }   
        public string ProjectName { get; set; }
        public bool IsApproved { get; set; }
        public string ReviewerUserName { get; set; }
        public int Rating { get; set; }        
        public string ReviewerFullName { get; set; }
        public string ReviewerCompanyName { get; set; }
        public string ReviewerDesignation { get; set; }
        public string CategoryName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Summary { get; set; }

    }
}
