using UpVotes.BusinessEntities.Entities;

namespace UpVotes.BusinessServices.Interface
{
    public interface ICompanyReviewsService
    {
        bool AddCompanyReview(CompanyReviewsEntity companyReviewEntity);
    }
}
