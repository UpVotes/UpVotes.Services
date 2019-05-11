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
    public class OverviewAndNewsController : ApiController
    {        
        private readonly IOverviewAndNewsService _overviewNewsServices;

        public OverviewAndNewsController(IOverviewAndNewsService submitReviewsService)
        {
            _overviewNewsServices = submitReviewsService;
        }        

        [HttpPost]
        [Route("api/GetSoftwareCompanyNews")]
        public HttpResponseMessage GetCompanySoftwareNews(OverviewNewsEntity newsrequest)
        {
            OverviewNewsResponse retval = new OverviewNewsResponse();
            try
            {               
                retval = _overviewNewsServices.GetCompanySoftwareNews(newsrequest);
                return Request.CreateResponse(HttpStatusCode.OK, retval);               
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [Route("api/GetSoftwareCompanyNewsByName")]
        public HttpResponseMessage GetSoftwareCompanyNewsByName(OverviewNewsEntity newsrequest)
        {
            OverviewNewsResponse retval = new OverviewNewsResponse();
            try
            {
                retval = _overviewNewsServices.GetSoftwareCompanyNewsByName(newsrequest);
                return Request.CreateResponse(HttpStatusCode.OK, retval);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpPost]
        [Route("api/IsNewsExists")]
        public HttpResponseMessage IsNewsExists(OverviewNewsEntity newsrequest)
        {           
            try
            {               
                return Request.CreateResponse(HttpStatusCode.OK, _overviewNewsServices.IsNewsExists(newsrequest.Title, newsrequest.WebsiteURL));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        [Route("api/SaveAdminNews")]
        public HttpResponseMessage SaveAdminNews(OverviewNewsEntity newsrequest)
        {
            try
            {
                int NewsID = _overviewNewsServices.SaveAdminNews(newsrequest);
                if (NewsID != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, NewsID);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, NewsID);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpPost]
        [Route("api/SaveUserNews")]
        public HttpResponseMessage SaveUserNews(OverviewNewsEntity newsrequest)
        {
            try
            {
                int NewsID = _overviewNewsServices.SaveUserNews(newsrequest);
                if (NewsID != 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, NewsID);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Conflict, NewsID);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
