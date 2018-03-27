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

        [HttpGet]
        [Route("api/GetUserCompanies/{userID}")]
        public HttpResponseMessage GetUserCompanies(int userID)
        {
            try
            {
                CompanyDetail company = _companyServices.GetUserCompanies(userID);
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
    }
}
