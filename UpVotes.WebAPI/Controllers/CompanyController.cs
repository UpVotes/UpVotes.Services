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
    public class CompanyController : ApiController
    {
        private readonly ICompanyService _companyServices;
        private readonly IReviewsService _ReviewsServices;

        public CompanyController(ICompanyService companyService, IReviewsService companyReviewsService)
        {
            _companyServices = companyService;
            _ReviewsServices = companyReviewsService;
        }

        [HttpPost]
        [Route("api/GetCompany")]
        public HttpResponseMessage GetCompany(CompanyFilterEntity companyFilter)
        {
            try
            {
                CompanyDetail company = _companyServices.GetAllCompanyDetails(companyFilter.CompanyName, companyFilter.MinRate, companyFilter.MaxRate, companyFilter.MinEmployee, companyFilter.MaxEmployee, companyFilter.SortBy, companyFilter.FocusAreaID, companyFilter.Location, companyFilter.SubFocusArea, companyFilter.UserID, companyFilter.PageNo, companyFilter.PageSize, companyFilter.OrderColumn);

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
        [Route("api/GetCompanyReviewsForListingPage")]
        public HttpResponseMessage GetUserReviewsForCompanyListingPage(CompanyFilterEntity companyFilter)
        {
            try
            {
                CompanySoftwareReviews companyReviews = _companyServices.GetReviewsForCompanyListingPage(companyFilter);

                if (companyReviews != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, companyReviews);
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
        [Route("api/VoteForCompany")]
        public HttpResponseMessage VoteForCompany(CompanyVoteEntity companyVote)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _companyServices.VoteForCompany(companyVote));
            }
            catch (Exception ex)
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

        [HttpPost]
        [Route("api/GetUserReviews")]
        public HttpResponseMessage GetUserReviews(CompanyFilterEntity filter)
        {
            try
            {
                CompanyDetail company = _companyServices.GetUserReviews(filter.CompanyName, filter.Rows);
                return Request.CreateResponse(HttpStatusCode.OK, company);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/GetAllCompanyPortfolio")]
        public HttpResponseMessage GetCompanyPortfolio(CompanyFilterEntity filter)
        {
            try
            {
                CompanyDetail company = _companyServices.GetAllCompanyPortfolioByName(filter.CompanyName, filter.Rows);
                return Request.CreateResponse(HttpStatusCode.OK, company);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/GetAllCompanyPortfolioByID")]
        public HttpResponseMessage GetCompanyPortfolioByID(CompanyFilterEntity filter)
        {
            try
            {
                List<CompanyPortFolioEntity> companyPortFolioObj = _companyServices.GetCompanyPortfolioByID(filter.CompanyID, filter.Rows);
                return Request.CreateResponse(HttpStatusCode.OK, companyPortFolioObj);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/GetPortfolioInfoByID")]
        public HttpResponseMessage GetPortfolioInfoByID(CompanyPortFolioEntity filter)
        {
            try
            {
                CompanyPortFolioEntity PortFolioObj = _companyServices.GetPortfolioInfoByID(filter.CompanyPortFolioID);
                return Request.CreateResponse(HttpStatusCode.OK, PortFolioObj);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/DeleteCompanyPortfolio")]
        public HttpResponseMessage DeleteCompanyPortfolio(CompanyPortFolioEntity filter)
        {
            try
            {
                int deleted = _companyServices.DeleteCompanyPortfolio(filter.CompanyPortFolioID);
                return Request.CreateResponse(HttpStatusCode.OK, deleted);
            }
            catch (Exception ex)
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

        [HttpPost]
        [Route("api/InsertVerifyClaimListing")]
        public HttpResponseMessage InsertUpdateClaimListing(ClaimApproveRejectListingRequest claimrequest)
        {   
            try
            {
                string message = _companyServices.InsertUpdateClaimListing(claimrequest);
                return Request.CreateResponse(HttpStatusCode.OK, message);               
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }

        }

        [HttpPost]
        [Route("api/AdminApproveRejectForClaim")]
        public string AdminApproveRejectForClaiming(ClaimApproveRejectListingRequest claimrequest)
        {
            try
            {                
                string status = _companyServices.AdminApproveRejectForClaiming(claimrequest);
                return status;
            }
            catch (Exception ex)
            {
                return "error";
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

        [HttpGet]
        [Route("api/GetServiceCategoryLinks/{FocusAreaID}")]
        public HttpResponseMessage GetServiceCategoryLinks(int FocusAreaID)
        {
            try
            {
                List<CategoryLinksEntity> allServiceCategoryLinks = _companyServices.GetServiceCategoryLinks(FocusAreaID);
                return Request.CreateResponse(HttpStatusCode.OK, allServiceCategoryLinks);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/SaveCompany")]
        public HttpResponseMessage SaveCompany(CompanyEntity companyEntity)
        {
            try
            {
                int companyID = _companyServices.SaveCompany(companyEntity);
                if (companyID != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, companyID);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, companyID);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/SaveUpdateCompanyPortFolio")]
        public HttpResponseMessage SaveUpdateCompanyPortFolio(CompanyPortFolioEntity portfolioEntity)
        {
            try
            {
                int portfolioID = _companyServices.SaveUpdateCompanyPortFolio(portfolioEntity);
                if (portfolioID != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, portfolioID);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, portfolioID);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet, Route("api/GetTeamMembersByCompany/{companyName}")]        
        public HttpResponseMessage GetTeamMembersByCompany(string companyName)
        {
            try
            {                
                return Request.CreateResponse(HttpStatusCode.OK, _companyServices.GetTeamMembersByCompany(companyName));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost, Route("api/SaveCompanyTeamMembers")]
        public HttpResponseMessage SaveCompanyTeamMembers(TeamMemebersEntity teamMembers)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _companyServices.SaveCompanyTeamMembers(teamMembers));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost]
        [Route("api/DeleteCompanyTeamMember")]
        public HttpResponseMessage DeleteCompanyTeamMember(TeamMemebersEntity filter)
        {
            try
            {
                int deleted = _companyServices.DeleteCompanyTeamMember(filter.MemberId);
                return Request.CreateResponse(HttpStatusCode.OK, deleted);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet, Route("api/GetCompanyTeamMember/{teamMemberId}")]
        public HttpResponseMessage GetCompanyTeamMember(int teamMemberId)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _companyServices.GetCompanyTeamMember(teamMemberId));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }
    }
}
