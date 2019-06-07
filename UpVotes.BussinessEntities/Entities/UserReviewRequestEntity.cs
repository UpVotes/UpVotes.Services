using System;

namespace UpVotes.BusinessEntities.Entities
{
    public class UserReviewRequestEntity
    {
        public int userID { get; set; }
        public int companyID { get; set; }
        public int softwareID { get; set; }
        public int ReviewID { get; set; }
        public bool IsApprove { get; set; }

    }
}
