using System;

namespace UpVotes.BusinessEntities.Entities
{
    public class SoftwareReviewsEntity
    {
        public int SoftwareReviewID { get; set; }

        public int SoftwareID { get; set; }
        public string SoftwareName { get; set; }
        public int ServiceCategoryID { get; set; }
        public string ReviewerProjectName { get; set; }

        public string ReviewerSoftwareName { get; set; }

        public string FeedBack { get; set; }

        public int Rating { get; set; }

        public int UserID { get; set; }

        public string ReviewerFullName { get; set; }

        public string ReviewerCompanyName { get; set; }

        public string ReviewerDesignation { get; set; }

        public string ServiceCategoryName { get; set; }

        public DateTime ReviewDate { get; set; }

        public int NoOfThankNotes { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string UserProfilePicture { get; set; }
        public int NoOfReviews { get; set; }
    }
}
