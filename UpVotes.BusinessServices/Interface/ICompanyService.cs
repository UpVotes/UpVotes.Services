using System.Collections.Generic;
using UpVotes.BusinessEntities.Entities;

namespace UpVotes.BusinessServices.Interface
{
    public interface ICompanyService
    {
        CompanyDetail GetAllCompanyDetails(string companyName, decimal? minRate, decimal? maxRate, int? minEmployee, int? maxEmployee, string sortby, int? focusAreaID,string location, int userID = 0, int PageNo = 1,int PageSize = 10);

        CompanyDetail GetCompanyDetails(string companyName);

        bool InsertCompany(CompanyEntity companyEntity);

        bool UpdateCompany(int companyID, CompanyEntity companyEntity);

        bool DeleteCompany(int companyID);

        IEnumerable<AutoComplete> GetCompanyNames();

        string VoteForCompany(CompanyVoteEntity companyVote);

        string ThanksNoteForReview(CompanyReviewThankNoteEntity companyReviewThanksNoteEntity);
    }
}
