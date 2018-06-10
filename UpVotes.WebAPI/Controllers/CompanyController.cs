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
        [Route("api/GetCompany/{companyName}/{minRate}/{maxRate}/{minEmployee}/{maxEmployee}/{sortby}/{focusAreaID}/{location}/{SubFocusArea}/{userID}/{PageNo}/{PageSize}")]
        public HttpResponseMessage GetCompany(string companyName, decimal? minRate, decimal? maxRate, int? minEmployee, int? maxEmployee, string sortby, int? focusAreaID, string location, string SubFocusArea = "0", int userID = 0, int PageNo = 1, int PageSize = 10)
        {
            try
            {
                CompanyDetail company = _companyServices.GetAllCompanyDetails(companyName, minRate, maxRate, minEmployee, maxEmployee, sortby, focusAreaID,location, SubFocusArea, userID, PageNo, PageSize);
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
        [Route("api/GetDataForAutoComplete/{type}/{focusAreaID}/{searchTerm}")]
        public HttpResponseMessage GetDataForAutoComplete(int type, int focusAreaID, string searchTerm)
        {
            try
            {
                List<string> autoCompleteList = _companyServices.GetDataForAutoComplete(type, focusAreaID, searchTerm);
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
        [Route("api/GetUserReviews/{companyName}")]
        public HttpResponseMessage GetUserReviews(string companyName)
        {
            try
            {
                CompanyDetail company = _companyServices.GetUserReviews(companyName);
                return Request.CreateResponse(HttpStatusCode.OK, company);
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }
        [HttpPost]
        [Route("api/GetQuotationDetailsForMobileApp")]
        public HttpResponseMessage GetQuotationDetails(QuotationRequest quotationrequest)
        {

            var retval = new ServiceResponse<QuotationResponse> { ResponseCode = 0, ResponseDescription = "Success" };
            try
            {
                retval.ResponseObject = _companyServices.GetQuotationData(quotationrequest);
                if (retval.ResponseObject != null)
                {                    
                    return Request.CreateResponse(HttpStatusCode.OK, retval);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Quotation not found");
                }
            }
            catch (Exception ex)
            {                
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }            

        }

        [HttpGet]
        [Route("api/GetCategoryMetaTags/{FocusAreaName}/{SubFocusAreaName}")]
        public HttpResponseMessage GetCategoryMetaTags(string FocusAreaName, string SubFocusAreaName)
        {   
            try
            {
                CategoryMetaTags metaTagDetails = _companyServices.GetCategoryMetaTags(FocusAreaName, SubFocusAreaName);
                return Request.CreateResponse(HttpStatusCode.OK, metaTagDetails);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }

        }
    }
}
