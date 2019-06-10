using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessServices.Interface;

namespace UpVotes.WebAPI.Controllers
{
    //[AuthorizationRequired]
    public class SoftwareController : ApiController
    {
        private readonly ISoftwareService _softwareServices;       

        public SoftwareController(ISoftwareService softwareService)
        {
            _softwareServices = softwareService;            
        }

        [HttpPost, Route("api/GetSoftware")]
        public HttpResponseMessage GetSoftware(SoftwareFilterEntity softwareFilter)
        {
            try
            {
                SoftwareDetail software = _softwareServices.GetAllSoftwareDetails(softwareFilter.SoftwareCategoryId, softwareFilter.SoftwareName, softwareFilter.SortBy, softwareFilter.UserID, softwareFilter.PageNo, softwareFilter.PageSize);

                if (software != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, software);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Software not found");
                }
            }
            catch (Exception ex)
            {                
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [Route("api/GetUserReviewsForSoftware")]
        public HttpResponseMessage GetUserReviews(SoftwareFilterEntity filter)
        {
            try
            {
                SoftwareDetail software = _softwareServices.GetUserReviewsForSoftware(filter.SoftwareName, filter.Rows);
                return Request.CreateResponse(HttpStatusCode.OK, software);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/ThanksNoteForSoftwareReview")]
        public HttpResponseMessage ThanksNoteForSoftwareReview(SoftwareReviewThankNoteEntity softwareReviewThanksNoteEntity)
        {
            try
            {
                string message = _softwareServices.ThanksNoteForSoftwareReview(softwareReviewThanksNoteEntity);
                return Request.CreateResponse(HttpStatusCode.OK, message);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/GetSoftwareReviewsForListingPage")]
        public HttpResponseMessage GetUserReviewsForSoftwareListingPage(SoftwareFilterEntity softwareFilter)
        {
            try
            {
                CompanySoftwareReviews softwareReviews = _softwareServices.GetReviewsForSoftwareListingPage(softwareFilter);

                if (softwareReviews != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, softwareReviews);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Companies not found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [Route("api/VoteForSoftware")]
        public HttpResponseMessage VoteForSoftware(SoftwareVoteEntity softwareVote)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _softwareServices.VoteForSoftware(softwareVote));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/InsertVerifyClaimSoftwareListing")]
        public HttpResponseMessage InsertUpdateClaimSoftwareListing(ClaimApproveRejectListingRequest claimrequest)
        {
            try
            {
                string message = _softwareServices.InsertUpdateClaimSoftwareListing(claimrequest);
                return Request.CreateResponse(HttpStatusCode.OK, message);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpGet]
        [Route("api/GetSoftwareForAutoComplete/{SoftwareCategory}/{searchTerm}")]
        public HttpResponseMessage GetSoftwareForAutoComplete(int SoftwareCategory, string searchTerm)
        {
            try
            {
                List<string> autoCompleteList = _softwareServices.GetSoftwareForAutoComplete(SoftwareCategory, searchTerm);
                if (autoCompleteList.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, autoCompleteList);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }
        [HttpGet]
        [Route("api/GetSoftwareCategoryMetaTags/{SoftwareCategoryName}")]
        public HttpResponseMessage GetSoftwareCategoryMetaTags(string SoftwareCategoryName)
        {
            try
            {
                CategoryMetaTags metaTagDetails = _softwareServices.GetSoftwareCategoryMetaTags(SoftwareCategoryName);
                return Request.CreateResponse(HttpStatusCode.OK, metaTagDetails);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet, Route("api/GetUserSoftwares/{userId}/{isAdmin}")]
        public HttpResponseMessage GetUserSoftwares(int userId, bool isAdmin)
        {
            try
            {
                var userSoftwares = _softwareServices.GetUserSoftwaresByUserId(userId, isAdmin);
                return userSoftwares != null ? Request.CreateResponse(HttpStatusCode.OK, userSoftwares) : Request.CreateResponse(HttpStatusCode.NotFound, "No softwares found");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet, Route("api/GetUserSoftwareByName/{softwareName}")]
        public HttpResponseMessage GetUserSoftwareByName(string softwareName)
        {
            try
            {
                var userSoftware = _softwareServices.GetUserSoftwareByName(softwareName);
                return userSoftware != null ? Request.CreateResponse(HttpStatusCode.OK, userSoftware) : Request.CreateResponse(HttpStatusCode.NotFound, "No software found");
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, e);
            }
        }

        [HttpPost, Route("api/SaveSoftwareDetails")]
        public HttpResponseMessage SaveSoftwareDetails(SoftwareEntity softwareEntity)
        {
            try
            {
                int softwareId = _softwareServices.SaveSoftwareDetails(softwareEntity);
                if (softwareId != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, softwareId);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, softwareId);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet, Route("api/SoftwareVerificationByUser/{uId}/{cId}/{softId}")]
        public HttpResponseMessage SoftwareVerificationByUser(int uId, string cId, int softId)
        {
            try
            {
                bool isUserVerifiedSoftware = _softwareServices.SoftwareVerificationByUser(uId, cId, softId);
                if (isUserVerifiedSoftware)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, isUserVerifiedSoftware);
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, isUserVerifiedSoftware);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost, Route("api/UpdateSoftwareRejectionComments")]
        public HttpResponseMessage UpdateSoftwareRejectionComments(SoftwareRejectComments softwareRejectComments)
        {
            try
            {
                bool isRejectionCommentsUpdated = _softwareServices.UpdateSoftwareRejectionComments(softwareRejectComments);
                return Request.CreateResponse(HttpStatusCode.OK, isRejectionCommentsUpdated);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet, Route("api/GetTeamMembersBySoftware/{softwareName}")]
        public HttpResponseMessage GetTeamMembersBySoftware(string softwareName)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _softwareServices.GetTeamMembersBySoftware(softwareName));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost, Route("api/SaveSoftwareTeamMembers")]
        public HttpResponseMessage SaveSoftwareTeamMembers(TeamMemebersEntity teamMembers)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _softwareServices.SaveSoftwareTeamMembers(teamMembers));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpDelete, Route("api/DeleteSoftwareTeamMember/{teamMemberId}")]
        public HttpResponseMessage DeleteSoftwareTeamMember(int teamMemberId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _softwareServices.DeleteSoftwareTeamMember(teamMemberId));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet, Route("api/GetSoftwareTeamMember/{teamMemberId}")]
        public HttpResponseMessage GetCompanyTeamMember(int teamMemberId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _softwareServices.GetCompanyTeamMember(teamMemberId));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }
    }
}
