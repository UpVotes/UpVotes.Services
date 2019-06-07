using System.Collections.Generic;
using UpVotes.BusinessEntities.Entities;

namespace UpVotes.BusinessServices.Interface
{
    public interface IReviewsService
    {
        bool AddCompanyReview(CompanyReviewsEntity companyReviewEntity);
        bool AddSoftwareReview(SoftwareReviewsEntity companyReviewEntity);
        AutocompleteResponseEntity GetSoftwareCompanyAutoCompleteData(AutocompleteRequestEntity SoftwareCompanyAutoCompleteRequest);
        List<UserReviewsResponseEntity> GetUserReviewsListForApproval(UserReviewRequestEntity ReviewsRequestEntity);
        UserReviewsResponseEntity GetUserReviewByReviewID(UserReviewRequestEntity ReviewsRequestEntity);
        bool ApproveRejectUserReview(UserReviewRequestEntity ApproveRejectRequestEntity);
    }
}
