using System;
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
    public class CompanyController : ApiController
    {
        private readonly ICompanyService _companyServices;
        private readonly ICompanyReviewsService _companyReviewsServices;

        public CompanyController(ICompanyService companyService, ICompanyReviewsService companyReviewsService)
        {
            _companyServices = companyService;
            _companyReviewsServices = companyReviewsService;
        }

        [HttpGet]
        [Route("api/GetCompany/{companyName}")]
        public HttpResponseMessage GetCompany(string companyName)
        {
            try
            {
                CompanyDetail company = _companyServices.GetCompanyDetails(companyName);
                if (company != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, company);
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

        [HttpGet]        
        [Route("api/GetCompany/{companyName}/{minRate}/{maxRate}/{minEmployee}/{maxEmployee}/{sortby}/{focusAreaID}/{location}/{userID}")]
        public HttpResponseMessage GetCompany(string companyName, decimal? minRate, decimal? maxRate, int? minEmployee, int? maxEmployee, string sortby, int? focusAreaID, string location, int userID = 0)
        {
            try
            {
                CompanyDetail company = _companyServices.GetAllCompanyDetails(companyName, minRate, maxRate, minEmployee, maxEmployee, sortby, focusAreaID,location, userID);
                if (company != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, company);
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
        [Route("api/SaveCompanyReview")]
        public HttpResponseMessage SaveCompanyReview(CompanyReviewsEntity companyReviewsEntity)
        {
            try
            {
                bool isSuccess = _companyReviewsServices.AddCompanyReview(companyReviewsEntity);
                return Request.CreateResponse(HttpStatusCode.OK, isSuccess);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/VoteForCompany")]
        public HttpResponseMessage VoteForCompany(CompanyVoteEntity companyVote)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _companyServices.VoteForCompany(companyVote));
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/ThanksNoteForReview")]
        public HttpResponseMessage ThanksNoteForReview(CompanyReviewThankNoteEntity companyReviewThanksNoteEntity)
        {
            try
            {
                string message = _companyServices.ThanksNoteForReview(companyReviewThanksNoteEntity);
                return Request.CreateResponse(HttpStatusCode.OK, message);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet]
        [Route("api/GetCompanyNames")]
        public HttpResponseMessage GetCompanyNames()
        {
            try
            {
                var companyNames = _companyServices.GetCompanyNames();
                if (companyNames.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, companyNames);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Company Names not found");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
