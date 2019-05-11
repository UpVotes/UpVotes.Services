﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UpVotes.BusinessEntities.Entities;
using UpVotes.BusinessServices.Interface;
using UpVotes.BusinessServices.Service;
using UpVotes.DataModel;
using UpVotes.WebAPI.Filters.Authentication;
using UpVotes.WebAPI.Filters.Authorization;

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

        [HttpPost]
        [Route("api/GetSoftware")]
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
    }
}
