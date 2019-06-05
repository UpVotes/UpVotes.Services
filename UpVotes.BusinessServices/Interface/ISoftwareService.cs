using System.Collections.Generic;
using UpVotes.BusinessEntities.Entities;

namespace UpVotes.BusinessServices.Interface
{
    public interface ISoftwareService
    {
        SoftwareDetail GetAllSoftwareDetails(int? serviceCategoryID, string softwareName, string sortby,int userID = 0, int PageNo = 1, int PageSize = 10);

        string VoteForSoftware(SoftwareVoteEntity softwareVote);

        List<string> GetSoftwareForAutoComplete(int SoftwareCategory, string searchTerm);

        CategoryMetaTags GetSoftwareCategoryMetaTags(string SoftwareCategoryAreaName);

        string ThanksNoteForSoftwareReview(SoftwareReviewThankNoteEntity softwareReviewThanksNoteEntity);

        string InsertUpdateClaimSoftwareListing(ClaimApproveRejectListingRequest ClaimSoftwareListingobj);

        CompanySoftwareReviews GetReviewsForSoftwareListingPage(SoftwareFilterEntity filter);

        SoftwareDetail GetUserReviewsForSoftware(string softwareName, int noOfRows);

        SoftwareDetail GetUserSoftwaresByUserId(int userId, bool isAdmin);

        SoftwareDetail GetUserSoftwareByName(string softwareName);

        int SaveSoftwareDetails(SoftwareEntity softwareEntity);

        bool SoftwareVerificationByUser(int uId, string cId, int softId);

        bool UpdateSoftwareRejectionComments(SoftwareRejectComments softwareRejectComments);

        List<TeamMemebersEntity> GetTeamMembersBySoftwareId(int softwareId);

        int SaveSoftwareTeamMembers(TeamMemebersEntity teamMembers);

        int DeleteSoftwareTeamMember(int teamMemberId);

        TeamMemebersEntity GetCompanyTeamMember(int teamMemberId);
    }
}
