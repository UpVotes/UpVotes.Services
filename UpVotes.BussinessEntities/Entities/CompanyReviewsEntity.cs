using System;

namespace UpVotes.BusinessEntities.Entities
{
    public class CompanyReviewsEntity
    {
        public int CompanyReviewID { get; set; }

        public int CompanyID { get; set; }

        public int FocusAreaID { get; set; }

        public string ReviewerProjectName { get; set; }

        public string FeedBack { get; set; }

        public int Rating { get; set; }

        public int UserID { get; set; }

        public string ReviewerFullName { get; set; }

        public string ReviewerCompanyName { get; set; }

        public string ReviewerDesignation { get; set; }

        public string FocusAreaName { get; set; }

        public DateTime ReviewDate { get; set; }

        public int NoOfThankNotes { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string UserProfilePicture { get; set; }
    }
}
