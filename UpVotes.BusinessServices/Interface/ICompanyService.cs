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
        CompanyDetail GetClaimListingsForApproval(int userID);

        List<string> GetDataForAutoComplete(int type, int focusAreaID, string searchTerm);

        CompanyDetail GetUserReviews(string companyName, int noOfRows);
        CompanyDetail GetAllCompanyPortfolioByName(string companyName, int noOfRows);

        List<CompanyPortFolioEntity> GetCompanyPortfolioByID(int companyID, int noOfRows);
        CompanyPortFolioEntity GetPortfolioInfoByID(int CompanyPortFolioID);

        int SaveUpdateCompanyPortFolio(CompanyPortFolioEntity portfolioobj);

        int DeleteCompanyPortfolio(int portfolioID);

        CompanySoftwareReviews GetReviewsForCompanyListingPage(CompanyFilterEntity filter);

        QuotationResponse GetQuotationData(QuotationRequest Quotationobj);

        List<CountryEntity> GetCountry();

        List<CompanyEntity> GetTopVoteCompanies();

        List<StateEntity> GetStates(int countryID);
        List<SubFocusAreaEntity> GetSubFocusAreaByFocusID(int FocusID);

        CategoryMetaTags GetCategoryMetaTags(string FocusAreaName, string SubFocusAreaName);
        List<CategoryLinksEntity> GetServiceCategoryLinks(int focusAreaID);

        bool CompanyVerificationByUser(int uID, string cID, int compID);
        bool UpdateRejectionComments(CompanyRejectComments companyRejectComments);
        string InsertUpdateClaimListing(ClaimApproveRejectListingRequest ClaimListingobj);
        string AdminApproveRejectForClaiming(ClaimApproveRejectListingRequest ClaimListingobj);
    }
}
