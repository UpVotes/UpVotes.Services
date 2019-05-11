using System.Collections.Generic;

namespace UpVotes.BusinessEntities.Entities
{
    public class CompanySoftwareReviews
    {
        public List<CompanyReviewsEntity> ReviewsList { get; set; }
        public List<string> CompanyNamesList { get; set; }
        public List<SoftwareReviewsEntity> SoftwareReviewsList { get; set; }
        public List<string> SoftwareNamesList { get; set; }

    }
}
