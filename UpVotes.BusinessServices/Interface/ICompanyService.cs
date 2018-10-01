using System.Collections.Generic;
using UpVotes.BusinessEntities.Entities;

namespace UpVotes.BusinessServices.Interface
{
    public interface ICompanyService
    {
        CompanyDetail GetAllCompanyDetails(string companyName, decimal? minRate, decimal? maxRate, int? minEmployee, int? maxEmployee, string sortby, int? focusAreaID, string location, string subFocusArea = "0", int userID = 0, int PageNo = 1, int PageSize = 10);

        int SaveCompany(CompanyEntity companyEntity);

        bool DeleteCompany(int companyID);

        string VoteForCompany(CompanyVoteEntity companyVote);

        string ThanksNoteForReview(CompanyReviewThankNoteEntity companyReviewThanksNoteEntity);

        CompanyDetail GetUserCompanies(int userID, string companyName);

        List<string> GetDataForAutoComplete(int type, int focusAreaID, string searchTerm);

        CompanyDetail GetUserReviews(string companyName);

        QuotationResponse GetQuotationData(QuotationRequest Quotationobj);

        List<CountryEntity> GetCountry();

        List<StateEntity> GetStates(int countryID);

        CategoryMetaTags GetCategoryMetaTags(string FocusAreaName, string SubFocusAreaName);

        bool CompanyVerificationByUser(int uID, string cID, int compID);
        bool UpdateRejectionComments(CompanyRejectComments companyRejectComments);
        string InsertUpdateClaimListing(ClaimApproveRejectListingRequest ClaimListingobj);
        string AdminApproveRejectForClaiming(ClaimApproveRejectListingRequest ClaimListingobj);
    }
}
