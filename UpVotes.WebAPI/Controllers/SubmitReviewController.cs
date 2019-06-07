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
    public class SubmitReviewController : ApiController
    {        
        private readonly IReviewsService _submitReviewsServices;

        public SubmitReviewController(IReviewsService submitReviewsService)
        {
            _submitReviewsServices = submitReviewsService;
        }

        

        [HttpPost]
        [Route("api/GetSoftwareCompanyAutoComplete")]
        public HttpResponseMessage GetSoftwareCompanyNameAutoComplete(AutocompleteRequestEntity autocompleterequest)
        {
            AutocompleteResponseEntity retval = new AutocompleteResponseEntity();
            try
            {               
                retval = _submitReviewsServices.GetSoftwareCompanyAutoCompleteData(autocompleterequest);
                return Request.CreateResponse(HttpStatusCode.OK, retval);               
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [Route("api/SaveCompanyReview")]
        public HttpResponseMessage SaveCompanyReview(CompanyReviewsEntity companyReviewsEntity)
        {
            try
            {
                bool isSuccess = _submitReviewsServices.AddCompanyReview(companyReviewsEntity);
                return Request.CreateResponse(HttpStatusCode.OK, isSuccess);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/SaveSoftwareReview")]
        public HttpResponseMessage SaveSoftwareReview(SoftwareReviewsEntity softwareReviewsEntity)
        {
            try
            {
                bool isSuccess = _submitReviewsServices.AddSoftwareReview(softwareReviewsEntity);
                return Request.CreateResponse(HttpStatusCode.OK, isSuccess);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/GetUserReviewsListForApproval")]
        public HttpResponseMessage GetUserReviewsListForApproval(UserReviewRequestEntity ReviewsRequestEntity)
        {
            try
            {
                List<UserReviewsResponseEntity> reviewsApprovalList = _submitReviewsServices.GetUserReviewsListForApproval(ReviewsRequestEntity);
                return Request.CreateResponse(HttpStatusCode.OK, reviewsApprovalList);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/GetUserReviewByReviewID")]
        public HttpResponseMessage GetUserReviewByReviewID(UserReviewRequestEntity ReviewsRequestEntity)
        {
            try
            {
                UserReviewsResponseEntity reviewsApprovalList = _submitReviewsServices.GetUserReviewByReviewID(ReviewsRequestEntity);
                return Request.CreateResponse(HttpStatusCode.OK, reviewsApprovalList);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/ApproveRejectUserReview")]
        public HttpResponseMessage ApproveRejectUserReview(UserReviewRequestEntity ReviewsRequestEntity)
        {
            try
            {
                bool isApproveReject = _submitReviewsServices.ApproveRejectUserReview(ReviewsRequestEntity);
                return Request.CreateResponse(HttpStatusCode.OK, isApproveReject);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }
    }
}
