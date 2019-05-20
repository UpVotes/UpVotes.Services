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
    public class UserCompanyListController : ApiController
    {
        private readonly ICompanyService _companyServices;

        public UserCompanyListController(ICompanyService companyService)
        {
            _companyServices = companyService;
        }

        [HttpGet, Route("api/GetUserCompanies/{userID}/{companyName}")]
        public HttpResponseMessage GetUserCompanies(int userID, string companyName)
        {
            try
            {
                CompanyDetail company = _companyServices.GetUserCompanies(userID, companyName);
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
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [Route("api/GetClaimListingsForApproval/{userID}")]
        public HttpResponseMessage GetClaimListingsForApproval(int userID)
        {
            try
            {
                CompanyDetail company = _companyServices.GetClaimListingsForApproval(userID);
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
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet, Route("api/GetCountry")]
        public HttpResponseMessage GetCountry()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _companyServices.GetCountry());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet, Route("api/GetTopVoteCompanies")]
        public HttpResponseMessage GetTopVoteCompanies()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _companyServices.GetTopVoteCompanies());
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet, Route("api/GetStates/{countryID}")]
        public HttpResponseMessage GetStates(int countryID)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _companyServices.GetStates(countryID));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet, Route("api/GetSubFocusAreaByFocusID/{FocusAreaID}")]
        public HttpResponseMessage GetSubFocusAreaByFocusID(int FocusAreaID)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _companyServices.GetSubFocusAreaByFocusID(FocusAreaID));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpGet, Route("api/CompanyVerificationByUser/{uID}/{cID}/{compID}")]
        public HttpResponseMessage CompanyVerificationByUser(int uID, string cID, int compID)
        {
            try
            {
                bool isUserVerifiedCompany = _companyServices.CompanyVerificationByUser(uID, cID, compID);
                if (isUserVerifiedCompany)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, isUserVerifiedCompany);
                }

                return Request.CreateResponse(HttpStatusCode.BadRequest, isUserVerifiedCompany);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }

        [HttpPost, Route("api/UpdateRejectionComments")]
        public HttpResponseMessage UpdateRejectionComments(CompanyRejectComments companyRejectComments)
        {
            try
            {
                bool isRejectionCommentsUpdated = _companyServices.UpdateRejectionComments(companyRejectComments);
                return Request.CreateResponse(HttpStatusCode.OK, isRejectionCommentsUpdated);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex);
            }
        }        
    }
}
